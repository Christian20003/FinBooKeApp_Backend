using FinBookeAPI.DTO.Authentication.Input;
using FinBookeAPI.DTO.Authentication.Output;
using FinBookeAPI.DTO.Error;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Token;
using FinBookeAPI.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FinBookeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController(
    ILogger<AuthenticationController> logger,
    IAuthenticationService service
) : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger = logger;
    private readonly IAuthenticationService _service = service;

    /// <summary>
    /// This method process a login request and proofs if the user has access to his profile.
    /// </summary>
    /// <param name="data">
    /// The login data to authenticate the user.
    /// </param>
    /// <response code="200">If the user login was successful</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="403">If the user provided invalid credentials</response>
    /// <response code="423">If the user is locked due to incorrect login attemps</response>
    /// <response code="500">If any other kind of server error occur</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 423)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> Login([FromBody] LoginDTO data)
    {
        _logger.LogInformation(LogEvents.AuthenticationRequest, "Login request");
        var user = await _service.Login(data.Email, data.Password);
        return Ok(new UserDTO(user));
    }

    /// <summary>
    /// This method process a register request by generating a new user account.
    /// </summary>
    /// <param name="data">
    /// The data to create a new user account.
    /// </param>
    /// <response code="201">If the registration was successful</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="500">If any other kind of server error occur</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDTO), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> Register([FromBody] RegisterDTO data)
    {
        _logger.LogInformation(LogEvents.AuthenticationRequest, "Register request");
        var user = await _service.Register(data.Email, data.Name, data.Password);
        return Created(string.Empty, new UserDTO(user));
    }

    /// <summary>
    /// This method process a logout request by cancelling the current session.
    /// </summary>
    /// <param name="data">
    /// The data to verify the user for its logout attempt.
    /// </param>
    /// <response code="200">If the logout request was successful</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="403">If one of the provided tokens is invalid</response>
    /// <response code="500">If any other kind of server error occur</response>
    [HttpPost("logout")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> Logout([FromBody] LogoutDTO data)
    {
        _logger.LogInformation(LogEvents.AuthenticationRequest, "Logout request");
        await _service.Logout(data.AccessToken, data.RefreshToken);
        return Ok();
    }

    /// <summary>
    /// This method generates an access code that will be sent to the client's email address.
    /// </summary>
    /// <param name="data">
    /// The data to be able of sending a access code to the client.
    /// </param>
    /// <response code="200">If the security code has been generated and send successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="403">If the user provided an invalid email address (not assignable to any account)</response>
    /// <response code="500">If any other kind of server error occur</response>
    [HttpPost("forgotPwd")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPwdDTO data)
    {
        _logger.LogInformation(LogEvents.AuthenticationRequest, "Forgot password request");
        await _service.SendAccessCode(data.Email);
        return Ok();
    }

    /// <summary>
    /// This method resets a user account's password after validating the provided access code.
    /// The new password will be sent to the user via email.
    /// </summary>
    /// <param name="data">
    /// The data to generate a new password.
    /// </param>
    /// <response code="200">If the new password has been generated and send successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="403">
    /// If the user provided an invalid email address (not assignable to any account).
    /// If the provided security code is invalid or has expired.
    /// </response>
    /// <response code="500">If any other kind of server error occur</response>
    [HttpPost("resetPwd")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDTO data)
    {
        _logger.LogInformation(LogEvents.AuthenticationRequest, "Reset password request");
        await _service.ResetPassword(data.Email, data.AccessCode);
        return Ok();
    }

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
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 403)]
    [ProducesResponseType(typeof(ErrorResponse), 406)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
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
    }
}
