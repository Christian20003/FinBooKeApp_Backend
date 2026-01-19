using FinBookeAPI.AppConfig.Authentication;
using FinBookeAPI.Attributes;
using FinBookeAPI.DTO.CategoryType.Input;
using FinBookeAPI.DTO.CategoryType.Output;
using FinBookeAPI.DTO.Error;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Services.CategoryType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinBookeAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CategoryController(ICategoryService service, ILogger<CategoryController> logger)
    : ControllerBase
{
    private readonly ICategoryService _service = service;
    private readonly ILogger<CategoryController> _logger = logger;

    private string GetPath()
    {
        return $"{Request.Scheme}://{Request.Host.Value}";
    }

    /// <summary>
    /// This method returns a list of categories that corresponds
    /// to a specific user.
    /// </summary>
    /// <param name="nested">
    /// Whether the categories should be arranged in a nested structure
    /// or in a simple list.
    /// </param>
    /// <returns>
    /// All categories that corresponds to a specific user.
    /// </returns>
    /// <response code="200">If the categories could be read successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="500">If a server error occur</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> GetCategories([FromQuery] bool nested = false)
    {
        _logger.LogInformation(LogEvents.CategoryRequest, "Get request for categories");
        var userId = HttpContext.User.GetUserId();
        var categories = await _service.GetCategories(userId);
        if (nested)
        {
            var nestedCategories = _service.NestCategories(categories);
            return Ok(nestedCategories.Select(elem => new CategoryNestedDTO(elem, GetPath(), Url)));
        }
        return Ok(categories.Select(elem => new CategoryDTO(elem, GetPath(), Url)));
    }

    /// <summary>
    /// This method returns a specific category by id.
    /// </summary>
    /// <param name="id">
    /// The id of the category that should be returned.
    /// </param>
    /// <returns>
    /// The requested category.
    /// </returns>
    /// <response code="200">If the categories could be read successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="401">If the user does not have access to the requested category</response>
    /// <response code="404">If the requested category does not exist</response>
    /// <response code="500">If a server error occur</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> GetCategory(
        [Guid(ErrorMessage = "Id is not a valid GUID")] string id
    )
    {
        _logger.LogInformation(LogEvents.CategoryRequest, "Get request for category {id}", id);
        var categoryId = Guid.Parse(id);
        var userId = HttpContext.User.GetUserId();
        var result = await _service.GetCategory(categoryId, userId);
        return Ok(new CategoryDTO(result, GetPath(), Url));
    }

    /// <summary>
    /// This method creates a new category.
    /// </summary>
    /// <param name="category">
    /// The category that should be stored in the database.
    /// </param>
    /// <returns>
    /// The stored category.
    /// </returns>
    /// <response code="200">If the category could be added successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="401">If the user does not have access to specified child categories</response>
    /// <response code="500">If a server error occur</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDTO), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> CreateCategory(CreateCategoryDTO category)
    {
        _logger.LogInformation(LogEvents.CategoryRequest, "Post request for a category");
        var userId = HttpContext.User.GetUserId();
        var result = await _service.CreateCategory(category.GetCategory(userId));
        return CreatedAtAction(
            nameof(GetCategory),
            new { id = result.Id },
            new CategoryDTO(result, GetPath(), Url)
        );
    }

    /// <summary>
    /// This method updates an existing category.
    /// </summary>
    /// <param name="id">
    /// The id of the category that should be updated.
    /// </param>
    /// <param name="category">
    /// All modified properties of that category.
    /// </param>
    /// <returns>
    /// All categories that have been modified. This can include categories that were previously
    /// assigned as parents to categories that are now assigned as children to the updated category.
    /// </returns>
    /// <response code="200">If the category could be updated successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="401">
    /// If the user does not have access to specified child categories or the category itself.
    /// </response>
    /// <response code="404">If the requested category does not exist</response>
    /// <response code="500">If a server error occur</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> UpdateCategory(
        [Guid(ErrorMessage = "Id is not a valid GUID")] string id,
        UpdateCategoryDTO category
    )
    {
        _logger.LogInformation(LogEvents.CategoryRequest, "Put request for category {id}", id);
        var categoryId = Guid.Parse(id);
        var userId = HttpContext.User.GetUserId();
        var updatedCategory = await _service.UpdateCategory(category.GetCategory(userId));
        return Ok(updatedCategory.Select(elem => new CategoryDTO(elem, GetPath(), Url)));
    }

    /// <summary>
    /// This method deletes a category from the database.
    /// </summary>
    /// <param name="id">
    /// The id of the category that should be deleted.
    /// </param>
    /// <returns>
    /// The deleted category.
    /// </returns>
    /// <response code="200">If the category could be deleted successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="401">If the user does not have access to the category.</response>
    /// <response code="404">If the requested category does not exist</response>
    /// <response code="500">If a server error occur</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CategoryDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 401)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> DeleteCategory(
        [Guid(ErrorMessage = "Id is not a valid GUID")] string id
    )
    {
        _logger.LogInformation(LogEvents.CategoryRequest, "Delete request for category {id}", id);
        var categoryId = Guid.Parse(id);
        var userId = HttpContext.User.GetUserId();
        var result = await _service.DeleteCategory(categoryId, userId);
        return Ok(new CategoryDTO(result, GetPath(), Url));
    }
}
