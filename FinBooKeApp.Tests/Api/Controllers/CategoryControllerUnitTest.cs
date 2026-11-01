using FinBooKeApp.Api.Controllers.Categories;
using FinBooKeApp.Api.Controllers.Categories.Models;
using FinBooKeApp.Core.Services.Categories;
using FinBooKeApp.Core.Services.Categories.Models;
using FinBooKeApp.Tests.Mocks.Dependencies;
using FinBooKeApp.Tests.Mocks.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinBooKeApp.Tests.Api.Controllers;

public class CategoryControllerUnitTest
{
    private readonly Mock<ICategoryService> _service;
    private readonly CategoryController _controller;

    public CategoryControllerUnitTest()
    {
        var logger = new Mock<ILogger<CategoryController>>();
        var localizer = MockStringLocalizer.GetMock<CategoryController>();
        var claimProvider = MockClaimProvider.GetMock();
        var httpContext = MockHttpContext.GetMock();
        _service = MockCategoryService.GetMock();

        _controller = new CategoryController(
            _service.Object,
            claimProvider.Object,
            localizer.Object,
            logger.Object
        )
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext.Object },
        };
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

        Assert.IsType<BadRequestObjectResult>(response.Result);
    }

    public static IEnumerable<object[]> CreateCategoryStatusTestData()
    {
        yield return new object[]
        {
            CategoryServiceResultCode.CATEGORY_ALREADY_EXIST,
            StatusCodes.Status400BadRequest,
        };
        yield return new object[]
        {
            CategoryServiceResultCode.CHILD_CATEGORY_DOES_NOT_EXIST,
            StatusCodes.Status404NotFound,
        };
        yield return new object[]
        {
            CategoryServiceResultCode.PARENT_CATEGORY_DOES_NOT_EXIST,
            StatusCodes.Status404NotFound,
        };
        yield return new object[]
        {
            CategoryServiceResultCode.INVALID_CATEGORY_DEPENDENCY,
            StatusCodes.Status400BadRequest,
        };
        yield return new object[]
        {
            CategoryServiceResultCode.INVALID_CATEGORY_LIMIT,
            StatusCodes.Status400BadRequest,
        };
    }

    [Theory]
    [MemberData(nameof(CreateCategoryStatusTestData))]
    public async Task CreateCategory_WhenCategoryServiceReturnsErrorResultCode_ReturnCorrespondingStatusCode(
        CategoryServiceResultCode code,
        int expectedStatusCode
    )
    {
        var dto = GetCategoryRequestDTO();
        MockCategoryService.SetReturnValueOfCreateCategoryAsync(_service, code);

        var response = await _controller.CreateCategory(dto, new CancellationToken());

        var result = Assert.IsType<ObjectResult>(response.Result);
        Assert.Equal(expectedStatusCode, result.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WhenCategoryServiceReturnsValidResult_ReturnCreatedCategory()
    {
        var dto = GetCategoryRequestDTO();
        var serviceResult = MockCategoryService.GetCreateCategoryResult();
        MockCategoryService.SetReturnValueOfCreateCategoryAsync(_service, serviceResult);

        var response = await _controller.CreateCategory(dto, new CancellationToken());

        var result = Assert.IsType<CreatedAtActionResult>(response.Result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        var body = Assert.IsType<CategoryResponseDTO>(result.Value);
        Assert.Equal(serviceResult.CreatedCategory.Name, body.Category.Name);
    }

    [Fact]
    public async Task CreateCategory_WhenCategoryServiceReturnsValidResult_ReturnModifiedCategories()
    {
        var dto = GetCategoryRequestDTO();
        var serviceResult = MockCategoryService.GetCreateCategoryResult();
        MockCategoryService.SetReturnValueOfCreateCategoryAsync(_service, serviceResult);

        var response = await _controller.CreateCategory(dto, new CancellationToken());

        var result = Assert.IsType<CreatedAtActionResult>(response.Result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        var body = Assert.IsType<CategoryResponseDTO>(result.Value);
        Assert.NotEmpty(body.ModifiedCategories);
    }
}
