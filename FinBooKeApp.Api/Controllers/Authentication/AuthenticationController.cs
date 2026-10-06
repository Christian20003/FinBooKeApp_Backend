using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FinBooKeApp.Api;
using FinBooKeApp.Api.Controllers.Authentication.Models;
using FinBooKeApp.Api.Shared.Error;
using FinBooKeApp.Api.Shared.Logging;
using FinBooKeApp.Core.Services.Authentication;
using FinBooKeApp.Core.Services.Authentication.Models;
using FinBooKeApp.Core.Shared.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinBookeAPI.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public partial class AuthenticationController(
    ILogger<AuthenticationController> logger,
    IAuthenticationService service,
    IClaimProvider claimProvider
) : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger = logger;
    private readonly IAuthenticationService _service = service;
    private readonly IClaimProvider _claimProvider = claimProvider;

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserDTO), 200)]
    [ProducesResponseType(typeof(MultipleErrorDTO), 400)]
    [ProducesResponseType(typeof(SingleErrorDTO), 403)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
    public async Task<ActionResult> Login([FromBody] LoginDTO data)
    {
        LogRequest(nameof(Login), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.LoginAsync(data.Email, data.Password);

        if (result.HasValue)
            return Ok(GetUserDTO(result.Value!));

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType, "Password");
        return StatusCode(error.Status, error);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDTO), 201)]
    [ProducesResponseType(typeof(MultipleErrorDTO), 400)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
    public async Task<ActionResult> Register([FromBody] RegisterDTO data)
    {
        LogRequest(nameof(Register), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.RegisterAsync(data.Email, data.Username, data.Password);

        if (result.HasValue)
            return Created(string.Empty, GetUserDTO(result.Value!));

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType, "Password");
        return StatusCode(error.Status, error);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(SingleErrorDTO), 401)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
    public async Task<ActionResult> Logout()
    {
        LogRequest(nameof(Logout), Activity.Current?.Id);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.LogoutAsync(userId);

        if (result.HasValue)
            return Ok();

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType);
        return StatusCode(error.Status, error);
    }

    [AllowAnonymous]
    [HttpGet("resetPassword")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(MultipleErrorDTO), 400)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
    public async Task<ActionResult> ResetPasswordToken(
        [FromQuery]
        [EmailAddress(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            string email
    )
    {
        LogRequest(nameof(ResetPasswordToken), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.SendResetPasswordTokenAsync(email);

        if (result.HasValue)
            return Ok();

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType, "Email");
        return StatusCode(error.Status, error);
    }

    [AllowAnonymous]
    [HttpPost("resetPassword")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(MultipleErrorDTO), 400)]
    [ProducesResponseType(typeof(SingleErrorDTO), 403)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO data)
    {
        LogRequest(nameof(ResetPassword), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.ResetPasswordAsync(data.Email, data.NewPassword, data.Token);

        if (result.HasValue)
            return Ok();

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType, "NewPassword");
        return StatusCode(error.Status, error);
    }

    [AllowAnonymous]
    [HttpPost("accessToken")]
    [ProducesResponseType(typeof(SessionDTO), 200)]
    [ProducesResponseType(typeof(MultipleErrorDTO), 400)]
    [ProducesResponseType(typeof(SingleErrorDTO), 403)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
    public async Task<ActionResult<SessionDTO>> RefreshAccessToken(
        [FromBody] RefreshAccessTokenDTO data
    )
    {
        LogRequest(nameof(RefreshAccessToken), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.RefreshAccessTokenAsync(data.Email, data.RefreshToken);

        if (result.HasValue)
            return Ok(GetSessionDTO(result.Value!));

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType);
        return StatusCode(error.Status, error);
    }

    private static UserDTO GetUserDTO(User user)
    {
        return new UserDTO
        {
            Email = user.Email,
            Name = user.Name,
            ImagePath = user.ImagePath,
            Theme = user.Theme,
            Language = user.Language,
            Session = GetSessionDTO(user.Session),
        };
    }

    private static SessionDTO GetSessionDTO(Session session)
    {
        return new SessionDTO
        {
            AccessToken = session.AccessToken,
            AccessTokenExpiresAt = session.AccessTokenExpire,
            RefreshToken = session.RefreshToken,
            RefreshTokenExpiresAt = session.RefreshTokenExpire,
        };
    }

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRequest,
        Level = LogLevel.Information,
        Message = "Received {Type} request - {TraceId}"
    )]
    private partial void LogRequest(string type, string? traceId);
}
