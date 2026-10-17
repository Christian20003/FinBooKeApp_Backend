using FinBooKeApp.Api.Controllers.Category;
using FinBooKeApp.Api.Controllers.Category.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinBookeApp.Tests.Api.Controllers;

public class CategoryControllerUnitTest
{
    private readonly CategoryController _controller;

    public CategoryControllerUnitTest()
    {
        var logger = new Mock<ILogger<CategoryController>>();

        _controller = new CategoryController(logger.Object);
    }

    private static CreateCategoryDTO GetCreateCategoryDTO()
    {
        return new CreateCategoryDTO { Name = "Test Category", Color = "#ffffff" };
    }

    [Fact]
    public async Task CreateCategory_WhenModelStateIsInvalid_ReturnBadRequest()
    {
        var dto = GetCreateCategoryDTO();
        _controller.ModelState.AddModelError("Key", "Msg");

        var response = await _controller.CreateCategory(dto);

        var obj = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal(StatusCodes.Status400BadRequest, obj.StatusCode);
    }
}
