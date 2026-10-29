using FinBooKeApp.Api.Controllers.Categories;
using FinBooKeApp.Api.Controllers.Categories.Models;
using FinBooKeApp.Core.Services.Categories;
using FinBooKeApp.Core.Shared.Authentication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinBooKeApp.Tests.Api.Controllers;

public class CategoryControllerUnitTest
{
    private readonly CategoryController _controller;

    public CategoryControllerUnitTest()
    {
        var logger = new Mock<ILogger<CategoryController>>();
        var localizer = new Mock<IStringLocalizer<CategoryController>>();
        var claimProvider = new Mock<IClaimProvider>();
        var service = new Mock<ICategoryService>();

        _controller = new CategoryController(
            service.Object,
            claimProvider.Object,
            localizer.Object,
            logger.Object
        );
    }

    private static CategoryRequestDTO GetCategoryRequestDTO()
    {
        return new CategoryRequestDTO { Name = "Test Category", Color = "#ffffff" };
    }

    [Fact]
    public async Task CreateCategory_WhenModelStateIsInvalid_ReturnBadRequest()
    {
        var dto = GetCategoryRequestDTO();
        _controller.ModelState.AddModelError("Key", "Msg");

        var response = await _controller.CreateCategory(dto, new CancellationToken());

        var obj = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, obj.StatusCode);
    }
}
