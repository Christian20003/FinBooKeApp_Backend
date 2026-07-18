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

        var result = await _service.ResetPasswordAsync(data);

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

        var result = await _service.RefreshAccessTokenAsync(data);

        if (result.HasValue)
            return Ok(result.Value);

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType);
        return StatusCode(error.Status, error);
    }

    [LoggerMessage(
        EventId = LogEvents.AuthenticationRequest,
        Level = LogLevel.Information,
        Message = "Received {Type} request - {TraceId}"
    )]
    private partial void LogRequest(string type, string? traceId);
}
