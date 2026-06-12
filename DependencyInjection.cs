using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sportiva.Services;

namespace Career_Path
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<CancellationExceptionFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()
                );
            });

            services.AddOpenApi();

            services.AddCors(options =>
                options.AddDefaultPolicy(builder =>
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(
                        "http://localhost:5173",
                        "https://front-end-project-bay-seven.vercel.app"
                            )
                    .AllowCredentials()
                )
            );

            services.AddAuthConfig(configuration);

            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services
                .AddMapsterConfig()
                .AddFluentValidationConfig();
            services.AddSignalR();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddBackgroundJobsConfig(configuration);

            services.AddOptions<MailSettings>()
                .BindConfiguration(nameof(MailSettings))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }

        // ==================== Mapster ====================
        private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
        {
            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            mappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(mappingConfig));
            return services;
        }

        // ==================== FluentValidation ====================
        private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }

        // ==================== AUTH CONFIG ====================
        private static IServiceCollection AddAuthConfig(this IServiceCollection services,
     IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            services.AddSingleton<IJwtProvider, JwtProvider>();

            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtSettings = configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>();

            // ── Read OAuth config ───────────────────────────────────────────────
            var googleConfig = configuration
                .GetSection(GoogleOAuthOptions.SectionName)
                .Get<GoogleOAuthOptions>();

            var githubConfig = configuration
                .GetSection(GitHubOAuthOptions.SectionName)
                .Get<GitHubOAuthOptions>();

            // ── Bind options so they can be injected anywhere via IOptions<T> ──
            services.Configure<GoogleOAuthOptions>(
                configuration.GetSection(GoogleOAuthOptions.SectionName));

            services.Configure<GitHubOAuthOptions>(
                configuration.GetSection(GitHubOAuthOptions.SectionName));

            // ── Authentication pipeline ─────────────────────────────────────────
            var authBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings!.Key)),
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                };

                // ── SignalR JWT from Query String ───────────────────────────────
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            // ── Google OAuth (only if configured) ──────────────────────────────
            if (!string.IsNullOrWhiteSpace(googleConfig?.ClientId) &&
                !string.IsNullOrWhiteSpace(googleConfig?.ClientSecret))
            {
                authBuilder.AddGoogle(options =>
                {
                    options.ClientId = googleConfig.ClientId;
                    options.ClientSecret = googleConfig.ClientSecret;
                    options.SaveTokens = true;

                    if (!string.IsNullOrWhiteSpace(googleConfig.RedirectUri))
                        options.CallbackPath = googleConfig.RedirectUri;

                    foreach (var scope in googleConfig.Scopes ?? ["email", "profile"])
                        options.Scope.Add(scope);
                });
            }

            // ── GitHub OAuth (only if configured) ──────────────────────────────
            if (!string.IsNullOrWhiteSpace(githubConfig?.ClientId) &&
                !string.IsNullOrWhiteSpace(githubConfig?.ClientSecret))
            {
                authBuilder.AddGitHub(options =>
                {
                    options.ClientId = githubConfig.ClientId;
                    options.ClientSecret = githubConfig.ClientSecret;
                    options.CallbackPath = "/signin-github";
                    options.SaveTokens = true;

                    foreach (var scope in githubConfig.Scopes ?? ["user:email"])
                        options.Scope.Add(scope);
                });
            }

            // ── Prevent cookie redirects on API endpoints → return 401/403 ─────
            services.ConfigureApplicationCookie(options =>
            {
                options.Events = new Microsoft.AspNetCore.Authentication.Cookies
                    .CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") ||
                            ctx.Request.Headers["Accept"].ToString()
                               .Contains("application/json"))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }
                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") ||
                            ctx.Request.Headers["Accept"].ToString()
                               .Contains("application/json"))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return Task.CompletedTask;
                        }
                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });

            return services;
        }
        // ==================== Hangfire ====================
        private static IServiceCollection AddBackgroundJobsConfig(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    configuration.GetConnectionString("HangfireConnection")));

            services.AddHangfireServer();

            return services;
        }
    }
}