using System.Diagnostics;
using FinBooKeApp.Api.Controllers.Category.Models;
using FinBooKeApp.Api.Shared.Error;
using FinBooKeApp.Api.Shared.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinBooKeApp.Api.Controllers.Category;

[ApiController]
//[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public partial class CategoryController(ILogger<CategoryController> logger) : ControllerBase
{
    private readonly ILogger<CategoryController> _logger = logger;

    [HttpPost]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryDTO body)
    {
        LogRequest(nameof(CreateCategory), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        return Ok();
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryRequest,
        Level = LogLevel.Information,
        Message = "Received {Type} request - {TraceId}"
    )]
    private partial void LogRequest(string type, string? traceId);
}
