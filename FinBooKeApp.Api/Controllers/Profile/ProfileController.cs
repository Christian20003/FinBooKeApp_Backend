using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FinBooKeApp.Api.Controllers.Profile.Models;
using FinBooKeApp.Api.Shared.Error;
using FinBooKeApp.Api.Shared.Logging;
using FinBooKeApp.Api.Shared.Utils;
using FinBooKeApp.Core.Services.Profile;
using FinBooKeApp.Core.Shared.Authentication.Interfaces;
using FinBooKeApp.Core.Shared.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FinBooKeApp.Api.Controllers.Profile;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public partial class ProfileController(
    IProfileService service,
    IClaimProvider claimProvider,
    IStringLocalizer<ProfileController> localizer,
    ILogger<ProfileController> logger
) : ControllerBase
{
    private readonly IProfileService _service = service;
    private readonly IClaimProvider _claimProvider = claimProvider;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly ILogger<ProfileController> _logger = logger;

    private static readonly string INTERNAL_ERROR_KEY = "InternalError";
    private static readonly string EMAIL_IDENTICAL_KEY = "EmailIdentical";

    [HttpGet("email/change")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangeEmailToken(
        [FromQuery]
        [EmailAddress(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            string newEmail
    )
    {
        LogRequest(nameof(ChangeEmailToken), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.GetChangeEmailTokenAsync(Guid.Parse(userId), newEmail);

        if (result.HasValue())
            return Ok();

        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    [HttpPost("email/change")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ChangeEmail(
        [FromQuery]
        [EmailAddress(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            string newEmail,
        [FromQuery]
        [Required(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.Token),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            string token
    )
    {
        LogRequest(nameof(ChangeEmailToken), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.ChangeEmailAsync(Guid.Parse(userId), token, newEmail);
        if (result.HasValue())
            return Ok();
        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    [HttpDelete("image/{filename}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteImage(
        [FromRoute]
        [Required(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.Filename),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            string filename
    )
    {
        LogRequest(nameof(DeleteImage), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.DeleteProfileImageAsync(Guid.Parse(userId), filename);
        if (result.HasValue())
            return Ok();
        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    [HttpPost("image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> SetImage(
        [Required(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.File),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            IFormFile file
    )
    {
        LogRequest(nameof(SetImage), Activity.Current?.Id);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.SetProfileImageAsync(Guid.Parse(userId), new FileUpload(file));
        if (result.HasValue())
            return Ok(result.Value);
        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    [HttpGet("image/{filename}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<byte[]>> GetImage(
        [FromRoute]
        [Required(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.Filename),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            string filename
    )
    {
        LogRequest(nameof(GetImage), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = _service.GetProfileImage(Guid.Parse(userId), filename);
        return Ok(result);
    }

    [HttpGet("email/verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> VerifyEmailToken()
    {
        LogRequest(nameof(DeleteImage), Activity.Current?.Id);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.GetVerifyEmailTokenAsync(Guid.Parse(userId));
        if (result.HasValue())
            return Ok();
        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    [HttpPost("email/verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> VerifyEmail(
        [FromQuery]
        [Required(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.Token),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            string token
    )
    {
        LogRequest(nameof(VerifyEmail), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.VerifyEmailAsync(Guid.Parse(userId), token);
        if (result.HasValue())
            return Ok(result.Value);
        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    [HttpPost("language")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetLanguage(SetLanguageDTO data)
    {
        LogRequest(nameof(SetLanguage), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.SetProfileLanguageAsync(Guid.Parse(userId), data.LanguageType);
        if (result.HasValue())
            return Ok(result.Value);
        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    [HttpPost("theme")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetTheme(SetThemeDTO data)
    {
        LogRequest(nameof(SetTheme), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.SetProfileThemeAsync(Guid.Parse(userId), data.ThemeType);
        if (result.HasValue())
            return Ok(result.Value);
        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    [HttpPost("name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SetUsername(SetUsernameDTO data)
    {
        LogRequest(nameof(SetUsername), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.SetUsernameAsync(Guid.Parse(userId), data.Username);
        if (result.HasValue())
            return Ok(result.Value);
        var error = GetErrorDTO(result.ErrorCode);
        return StatusCode(error.Status, error);
    }

    private BaseErrorDTO GetErrorDTO(ServiceResultCode resultCode)
    {
        return resultCode switch
        {
            ServiceResultCode.TOKEN_INVALID or ServiceResultCode.USER_NOT_FOUND =>
                ErrorMapper.GetErrorDTO([""], ErrorType.FORBIDDEN),
            ServiceResultCode.EMAIL_IDENTICAL => ErrorMapper.GetErrorDTO(
                [_localizer.GetString(EMAIL_IDENTICAL_KEY)],
                ErrorType.BAD_REQUEST
            ),
            _ => ErrorMapper.GetErrorDTO(
                [_localizer.GetString(INTERNAL_ERROR_KEY)],
                ErrorType.INTERNAL_ERROR
            ),
        };
    }

    [LoggerMessage(
        EventId = LogEvents.ProfileRequest,
        Level = LogLevel.Information,
        Message = "Received {Type} request - {TraceId}"
    )]
    private partial void LogRequest(string type, string? traceId);
}
