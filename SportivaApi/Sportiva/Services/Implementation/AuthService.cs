using Hangfire;
using System.Text.RegularExpressions;

namespace Sportiva.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext context) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ApplicationDbContext _context = context;
    private readonly string _frontendOrigin = "https://front-end-project-bay-seven.vercel.app";
    private readonly int _refreshTokenExpiryDays = 14;

    // ════════════════════════════════════════════════════════════════════════
    //  OAuth — Google / GitHub
    // ════════════════════════════════════════════════════════════════════════

    public async Task<Result<AuthResponse>> HandleGoogleLoginAsync()
        => await HandleExternalLoginAsync();

    public async Task<Result<AuthResponse>> HandleGitHubLoginAsync()
        => await HandleExternalLoginAsync();

    private async Task<Result<AuthResponse>> HandleExternalLoginAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidExternalLogin);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidExternalLogin);

            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

            // GitHub doesn't provide GivenName/Surname — split the full name instead
            if (string.IsNullOrEmpty(firstName) && info.LoginProvider == "GitHub")
            {
                var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
                var nameParts = fullName.Split(' ', 2);
                firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            }

            // ── Try signing in with the external login directly ──────────────
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            ApplicationUser? user = null;

            if (signInResult.Succeeded)
            {
                // ✅ Resolve the user id first, then load with RefreshTokens
                // (never call .Result inside a LINQ expression — it deadlocks)
                var linkedUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (linkedUser is null)
                    return Result.Failure<AuthResponse>(UserErrors.InvalidExternalLogin);

                user = await _context.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.Id == linkedUser.Id, cancellationToken);
            }
            else
            {
                // ── Find or create the user ──────────────────────────────────
                user = await _context.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

                if (user is null)
                {
                    string username;

                    if (info.LoginProvider == "GitHub")
                    {
                        var githubUsername =
                            info.Principal.FindFirstValue("urn:github:login") ??
                            info.Principal.FindFirstValue("urn:github:name");

                        username = await GenerateUniqueUsernameAsync(email, githubUsername);
                    }
                    else
                    {
                        username = await GenerateUniqueUsernameAsync(email);
                    }

                    user = new ApplicationUser
                    {
                        UserName = username,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        EmailConfirmed = true   // OAuth provider already verified the email
                    };

                    var createResult = await _userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                    {
                        var createError = createResult.Errors.First();
                        return Result.Failure<AuthResponse>(
                            new Error(createError.Code, createError.Description,
                                      StatusCodes.Status400BadRequest));
                    }

                    await _userManager.AddToRoleAsync(user, DefaultRoles.Member.Name);

                    // Create the user profile — same as ConfirmEmailAsync
                    var userProfile = new UserProfile { UserId = user.Id };
                    _context.UserProfiles.Add(userProfile);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                await _userManager.AddLoginAsync(user, info);
            }

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidExternalLogin);

            // ── Common checks ────────────────────────────────────────────────
            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedUser);

            // ── Issue JWT + refresh token ────────────────────────────────────
            var (userRoles, userPermissions) =
                await GetUserRolesAndPermissions(user, cancellationToken);

            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(
                user.Id, user.Email, user.FirstName, user.LastName,
                token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during external login");
            return Result.Failure<AuthResponse>(UserErrors.UnexpectedError);
        }
    }

    private async Task<string> GenerateUniqueUsernameAsync(
        string email, string? preferredUsername = null)
    {
        if (!string.IsNullOrWhiteSpace(preferredUsername))
        {
            var sanitized = Regex.Replace(preferredUsername, @"[^a-zA-Z0-9_]", "");

            if (!string.IsNullOrWhiteSpace(sanitized))
            {
                if (await _userManager.FindByNameAsync(sanitized) is null)
                    return sanitized;

                var counter = 1;
                var candidate = $"{sanitized}{counter}";

                while (await _userManager.FindByNameAsync(candidate) is not null)
                    candidate = $"{sanitized}{++counter}";

                return candidate;
            }
        }

        var baseUsername = Regex.Replace(email.Split('@')[0], @"[^a-zA-Z0-9_]", "");

        if (string.IsNullOrWhiteSpace(baseUsername))
            baseUsername = "user";

        if (await _userManager.FindByNameAsync(baseUsername) is null)
            return baseUsername;

        var num = 1;
        var candidate2 = $"{baseUsername}{num}";

        while (await _userManager.FindByNameAsync(candidate2) is not null)
            candidate2 = $"{baseUsername}{++num}";

        return candidate2;
    }

    // ════════════════════════════════════════════════════════════════════════
    //  JWT Auth
    // ════════════════════════════════════════════════════════════════════════

    public async Task<Result<AuthResponse>> GetTokenAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

            if (result.Succeeded)
            {
                var (userRoles, userPermissions) =
                    await GetUserRolesAndPermissions(user, cancellationToken);

                var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpiration
                });

                await _userManager.UpdateAsync(user);

                var response = new AuthResponse(
                    user.Id, user.Email, user.FirstName, user.LastName,
                    token, expiresIn, refreshToken, refreshTokenExpiration);

                return Result.Success(response);
            }

            var error = result.IsNotAllowed ? UserErrors.EmailNotConfirmed
                      : result.IsLockedOut ? UserErrors.LockedUser
                                             : UserErrors.InvalidCredentials;

            return Result.Failure<AuthResponse>(error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating token for user {Email}", email);
            return Result.Failure<AuthResponse>(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(
        string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _jwtProvider.ValidateToken(token, validateLifetime: false);

            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedUser);

            var userRefreshToken = user.RefreshTokens
                .SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var (userRoles, userPermissions) =
                await GetUserRolesAndPermissions(user, cancellationToken);

            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(
                user.Id, user.Email, user.FirstName, user.LastName,
                newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while refreshing token");
            return Result.Failure<AuthResponse>(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _jwtProvider.ValidateToken(token, validateLifetime: false);

            if (userId is null)
                return Result.Failure(UserErrors.InvalidJwtToken);

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
                return Result.Failure(UserErrors.InvalidJwtToken);

            var userRefreshToken = user.RefreshTokens
                .SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (userRefreshToken is null)
                return Result.Failure(UserErrors.InvalidRefreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while revoking refresh token");
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  Registration & Email Confirmation
    // ════════════════════════════════════════════════════════════════════════

    public async Task<Result> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailIsExists = await _userManager.Users
                .AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExists)
                return Result.Failure(UserErrors.DuplicatedEmail);

            var user = request.Adapt<ApplicationUser>();
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                _logger.LogInformation("Confirmation code: {code}", code);

                SendConfirmationEmail(user, code);
                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description,
                StatusCodes.Status400BadRequest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering user {Email}", request.Email);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        try
        {
            if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
                return Result.Failure(UserErrors.InvalidCode);

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = request.Code;

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, DefaultRoles.Member.Name);
                var userProfile = new UserProfile { UserId = user.Id };
                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();
                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description,
                StatusCodes.Status400BadRequest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while confirming email for user {UserId}",
                request.UserId);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        try
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {code}", code);

            SendConfirmationEmail(user, code);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while resending confirmation email to {Email}", request.Email);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  Password Reset
    // ════════════════════════════════════════════════════════════════════════

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        try
        {
            if (await _userManager.FindByEmailAsync(email) is not { } user)
                return Result.Success();

            if (!user.EmailConfirmed)
                return Result.Failure(UserErrors.EmailNotConfirmed with
                {
                    StatusCode = StatusCodes.Status400BadRequest
                });

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Reset code: {code}", code);

            SendResetPasswordEmail(user, code);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while sending reset password code to {Email}", email);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || !user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);

            IdentityResult result;

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description,
                StatusCodes.Status401Unauthorized));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while resetting password for {Email}", request.Email);
            return Result.Failure(UserErrors.UnexpectedError);
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  Private Helpers
    // ════════════════════════════════════════════════════════════════════════

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private void SendConfirmationEmail(ApplicationUser user, string code)
    {
        var emailBody = EmailBodyBuilder.GenerateEmailBody(
            new Dictionary<string, string>
            {
            { "{{name}}", user.FirstName },
            { "{{action_url}}", $"{_frontendOrigin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            });

        BackgroundJob.Enqueue(() =>
            _emailSender.SendEmailAsync(
                user.Email!,
                "✅ Career Path : Email Confirmation",
                emailBody
            ));
    }

    private void SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var emailBody = ForgetPasswordBodyBuilder.GenerateEmailBody(
            new Dictionary<string, string>
            {
            { "{{name}}", user.FirstName },
            { "{{action_url}}", $"{_frontendOrigin}/auth/forgetPassword?email={user.Email}&code={code}" }
            });

        BackgroundJob.Enqueue(() =>
            _emailSender.SendEmailAsync(
                user.Email!,
                "✅ Career Path: Reset Password",
                emailBody
            ));
    }

    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)>
        GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userPermissions = await (
            from r in _context.Roles
            join p in _context.RoleClaims on r.Id equals p.RoleId
            where userRoles.Contains(r.Name!)
            select p.ClaimValue!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return (userRoles, userPermissions);
    }
}