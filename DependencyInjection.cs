
namespace Sportiva;
public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers()
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
                    .AllowAnyOrigin()
            //.AllowCredentials()
            // .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
            )
        );

       // services.AddAuthConfig(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services
            .AddMapsterConfig()
            .AddFluentValidationConfig();
        //



        services.AddHttpClient();
        services.AddHttpContextAccessor();

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

        services.AddAuthentication(options =>
        {
            // JWT هو الافتراضي
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
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
                    Encoding.UTF8.GetBytes(jwtSettings!.Key)
                ),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
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
}