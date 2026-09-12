using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Mapping.Error;
using FinBookeAPI.Models.Configuration;
using FinBooKeAPI.Models.DTO.Error;
using FinBookeAPI.Services.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinBooKeAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public partial class ProfileController(
    IProfileService service,
    IClaimProvider claimProvider,
    ILogger<ProfileController> logger
) : ControllerBase
{
    private readonly IProfileService _service = service;
    private readonly IClaimProvider _claimProvider = claimProvider;
    private readonly ILogger<ProfileController> _logger = logger;

    [HttpGet("changeEmail")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(MultipleErrorDTO), 400)]
    [ProducesResponseType(typeof(SingleErrorDTO), 403)]
    [ProducesResponseType(typeof(SingleErrorDTO), 500)]
    public async Task<ActionResult> EmailChangeToken(
        [FromQuery]
        [EmailAddress(
            ErrorMessageResourceName = nameof(DataAnnotationValidation.Email),
            ErrorMessageResourceType = typeof(DataAnnotationValidation)
        )]
            string newEmail
    )
    {
        LogRequest(nameof(EmailChangeToken), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = _claimProvider.GetUserId(HttpContext.User);
        var result = await _service.GetChangeEmailTokenAsync(Guid.Parse(userId), newEmail);

        if (result.HasValue)
            return Ok();

        var error = ErrorMapper.GetErrorDTO(result.ErrorMessages, result.ErrorType);
        return StatusCode(error.Status, error);
    }

    [LoggerMessage(
        EventId = LogEvents.ProfileRequest,
        Level = LogLevel.Information,
        Message = "Received {Type} request - {TraceId}"
    )]
    private partial void LogRequest(string type, string? traceId);
}
