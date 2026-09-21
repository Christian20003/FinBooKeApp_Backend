using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Mapping.Error;
using FinBookeAPI.Models.Configuration;
using FinBooKeAPI.Models.DTO.Error;
using FinBookeAPI.Services.Profile;
using FinBooKeAPI.Services.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FinBooKeAPI.Controllers;

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

    [HttpGet("changeEmail")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(MultipleErrorDTO), 400)]
    [ProducesResponseType(typeof(SingleErrorDTO), 403)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
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

    [HttpPost("changeEmail")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(MultipleErrorDTO), 400)]
    [ProducesResponseType(typeof(SingleErrorDTO), 403)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
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

    private BaseErrorDTO GetErrorDTO(ServiceResultCode resultCode)
    {
        return resultCode switch
        {
            ServiceResultCode.TOKEN_INVALID or ServiceResultCode.USER_NOT_FOUND =>
                ErrorMapper.GetErrorDTO([""], FinBookeAPI.Models.Result.ErrorType.FORBIDDEN),
            ServiceResultCode.EMAIL_IDENTICAL => ErrorMapper.GetErrorDTO(
                [_localizer.GetString(EMAIL_IDENTICAL_KEY)],
                FinBookeAPI.Models.Result.ErrorType.BAD_REQUEST
            ),
            _ => ErrorMapper.GetErrorDTO(
                [_localizer.GetString(INTERNAL_ERROR_KEY)],
                FinBookeAPI.Models.Result.ErrorType.INTERNAL_ERROR
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
