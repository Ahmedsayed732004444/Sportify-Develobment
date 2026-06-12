
using Microsoft.AspNetCore.Authentication.Google;
using Sportiva.Services;
namespace Sportiva.Controllers;

[Route("auth")]
[ApiController]
[Produces("application/json")]
public class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger,
    IConfiguration configuration,
    SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly string _frontendOrigin =
        configuration["AppSettings:FrontendOrigin"] ?? "https://front-end-project-bay-seven.vercel.app";

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            GoogleDefaults.AuthenticationScheme,
            Url.Action(nameof(GoogleResponse))
        );
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await _authService.HandleGoogleLoginAsync();
        return HandleOAuthCallback(result);
    }

    [HttpGet("github-login")]
    public IActionResult GitHubLogin()
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            "GitHub",
            Url.Action(nameof(GitHubResponse))
        );
        return Challenge(properties, "GitHub");
    }

    [HttpGet("github-response")]
    public async Task<IActionResult> GitHubResponse()
    {
        var result = await _authService.HandleGitHubLoginAsync();
        return HandleOAuthCallback(result);
    }

    private IActionResult HandleOAuthCallback(Result<AuthResponse> result)
    {
        if (!result.IsSuccess)
            return Redirect($"{_frontendOrigin}/login?error=authentication_failed");

        var auth = result.Value;

        var redirectUrl = $"{_frontendOrigin}/oauth/callback" +
            $"?token={Uri.EscapeDataString(auth.Token)}" +
            $"&refreshToken={Uri.EscapeDataString(auth.RefreshToken)}" +
            $"&expiresIn={auth.ExpiresIn}" +
            $"&refreshTokenExpiration={Uri.EscapeDataString(auth.RefreshTokenExpiration.ToString())}" +
            $"&userId={Uri.EscapeDataString(auth.Id)}" +
            $"&email={Uri.EscapeDataString(auth.Email ?? "")}" +
            $"&firstName={Uri.EscapeDataString(auth.FirstName)}" +
            $"&lastName={Uri.EscapeDataString(auth.LastName)}";

        return Redirect(redirectUrl);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Logging with email: {email}", request.Email);
            var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.ConfirmEmailAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.ResendConfirmationEmailAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        try
        {
            var result = await _authService.SendResetPasswordCodeAsync(request.Email);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499);
        }
    }
}
