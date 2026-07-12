using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FinBooKeAPI;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Mapping.Error;
using FinBookeAPI.Models.Configuration;
using FinBooKeAPI.Models.DTO.Authentication;
using FinBooKeAPI.Models.DTO.Error;
using FinBookeAPI.Services.Authentication;
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

        var result = await _service.LoginAsync(data);

        if (result.HasValue)
            return Ok(result.Value);

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

        var result = await _service.RegisterAsync(data);

        if (result.HasValue)
            return Created(string.Empty, result.Value);

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
        var result = await _service.LogoutAsync(Guid.Parse(userId));

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

        var result = await _service.ResetPasswordAsync(data);

        if (result.HasValue)
            return Ok();

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType, "NewPassword");
        return StatusCode(error.Status, error);
    }

    /*
    /// <summary>
    /// This method generates a new access token after validating the provided refresh token.
    /// </summary>
    /// <param name="data">
    /// The data to be able of authenticate the user and generating a new access token.
    /// </param>
    /// <response code="200">If the new access token has been generated successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="403">If the provided refresh token is invalid or has expired.</response>
    /// <response code="500">If any other kind of server error occur</response>
    [HttpPost("refreshToken")]
    [ProducesResponseType(typeof(SessionDTO), 200)]
    [ProducesResponseType(typeof(BadRequestOldDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 403)]
    [ProducesResponseType(typeof(ErrorDTO), 406)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult<SessionDTO>> RefreshAccessToken(
        [FromBody] RefreshAccessTokenDTO data
    )
    {
        var token = await _service.IssueJwtToken(data.RefreshToken);
        var refreshToken = new JwtToken
        {
            Value = data.RefreshToken,
            Expires = data.RefreshTokenExpires,
        };
        return Ok(new SessionDTO(token, refreshToken));
    } */

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRequest,
        Level = LogLevel.Information,
        Message = "Received {Type} request - {TraceId}"
    )]
    private partial void LogRequest(string type, string? traceId);
}
