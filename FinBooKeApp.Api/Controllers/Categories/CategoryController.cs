using System.Diagnostics;
using FinBooKeApp.Api.Controllers.Categories.Models;
using FinBooKeApp.Api.Shared.Error;
using FinBooKeApp.Api.Shared.Logging;
using FinBooKeApp.Core.Services.Categories;
using FinBooKeApp.Core.Services.Categories.Models;
using FinBooKeApp.Core.Shared.Authentication.Interfaces;
using FinBooKeApp.Core.Shared.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FinBooKeApp.Api.Controllers.Categories;

[ApiController]
//[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public partial class CategoryController(
    ICategoryService service,
    IClaimProvider claimProvider,
    IStringLocalizer<CategoryController> localizer,
    ILogger<CategoryController> logger
) : ControllerBase
{
    private readonly ILogger<CategoryController> _logger = logger;
    private readonly IClaimProvider _claimProvider = claimProvider;
    private readonly IStringLocalizer<CategoryController> _localizer = localizer;
    private readonly ICategoryService _service = service;

    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MultipleErrorDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(SingleErrorDTO), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryResponseDTO>> CreateCategory(
        [FromBody] CategoryRequestDTO body,
        CancellationToken cancellationToken
    )
    {
        LogRequest(nameof(CreateCategory), Activity.Current?.Id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = _claimProvider.GetUserId(HttpContext.User);
        var param = new CreateCategoryParams
        {
            UserId = Guid.Parse(userId),
            Name = body.Name,
            Color = body.Color,
            ChildrenIds = body.Children,
            ParentId = body.Parent,
            LimitAmount = body.LimitAmount,
            LimitStartDate = body.LimitStart,
            LimitTimePeriod = body.LimitMonthlyPeriod,
        };

        var result = await _service.CreateCategoryAsync(param, cancellationToken);
        if (!result.HasValue())
        {
            var errorResponse = GetErrorDTO(result.ErrorCode);
            return StatusCode(errorResponse.Status, errorResponse);
        }
        var response = new CategoryResponseDTO(
            result.Value!.CreatedCategory,
            result.Value!.CreatedLimit,
            result.Value!.ModifiedCategories,
            result.Value!.ModifiedCategoryLimits
        );
        return CreatedAtAction(nameof(GetCategory), new { id = response.Category.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategory(Guid id)
    {
        return Ok();
    }

    private BaseErrorDTO GetErrorDTO(CategoryServiceResultCode errorCode)
    {
        switch (errorCode)
        {
            case CategoryServiceResultCode.CATEGORY_ALREADY_EXIST:
            {
                var text = _localizer.GetString("AlreadyExist");
                return ErrorMapper.GetErrorDTO([text], ErrorType.BAD_REQUEST, "Name");
            }
            case CategoryServiceResultCode.CHILD_CATEGORY_DOES_NOT_EXIST:
            {
                var text = _localizer.GetString("ChildDoesNotExist");
                return ErrorMapper.GetErrorDTO([text], ErrorType.NOT_FOUND);
            }
            case CategoryServiceResultCode.PARENT_CATEGORY_DOES_NOT_EXIST:
            {
                var text = _localizer.GetString("ParentDoesNotExist");
                return ErrorMapper.GetErrorDTO([text], ErrorType.NOT_FOUND);
            }
            case CategoryServiceResultCode.INVALID_CATEGORY_LIMIT:
            {
                var text = _localizer.GetString("InvalidLimit");
                return ErrorMapper.GetErrorDTO([text], ErrorType.BAD_REQUEST, "LimitAmount");
            }
            case CategoryServiceResultCode.INVALID_CATEGORY_DEPENDENCY:
            {
                var text = _localizer.GetString("InvalidDependency");
                return ErrorMapper.GetErrorDTO([text], ErrorType.BAD_REQUEST, "Children");
            }
            default:
            {
                var text = _localizer.GetString("ServerError");
                return ErrorMapper.GetErrorDTO([text], ErrorType.INTERNAL_ERROR);
            }
        }
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryRequest,
        Level = LogLevel.Information,
        Message = "Received {Type} request - {TraceId}"
    )]
    private partial void LogRequest(string type, string? traceId);
}
