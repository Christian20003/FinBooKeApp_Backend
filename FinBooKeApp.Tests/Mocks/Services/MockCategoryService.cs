using FinBooKeApp.Core.Services.Categories;
using FinBooKeApp.Core.Services.Categories.Models;
using FinBooKeApp.Core.Shared.Result;
using FinBooKeApp.Data.Models;
using Moq;

namespace FinBooKeApp.Tests.Mocks.Services;

public static class MockCategoryService
{
    public static Mock<ICategoryService> GetMock()
    {
        return new Mock<ICategoryService>();
    }

    public static CreateCategoryResult GetCreateCategoryResult()
    {
        return new CreateCategoryResult
        {
            CreatedCategory = new Category
            {
                Id = Guid.Empty,
                UserId = Guid.Empty,
                Name = "TestCategory",
                Color = "#ffffff",
            },
            CreatedLimit = new CategoryLimit
            {
                CategoryId = Guid.Empty,
                Amount = 500.0m,
                Start = DateTime.UtcNow,
                Period = TimePeriod.ONE_MONTH,
            },
            ModifiedCategories =
            [
                new Category
                {
                    Id = Guid.Empty,
                    UserId = Guid.Empty,
                    Name = "TestParentCategory",
                    Color = "#000000",
                },
            ],
            ModifiedCategoryLimits =
            [
                new CategoryLimit
                {
                    CategoryId = Guid.Empty,
                    Amount = 1000.0m,
                    Start = DateTime.UtcNow,
                    Period = TimePeriod.ONE_MONTH,
                },
            ],
        };
    }

    public static void SetReturnValueOfCreateCategoryAsync(
        Mock<ICategoryService> mock,
        CategoryServiceResultCode code
    )
    {
        mock.Setup(obj =>
                obj.CreateCategoryAsync(
                    It.IsAny<CreateCategoryParams>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new Result<CreateCategoryResult, CategoryServiceResultCode>(code));
    }

    public static void SetReturnValueOfCreateCategoryAsync(
        Mock<ICategoryService> mock,
        CreateCategoryResult categoryResult
    )
    {
        mock.Setup(obj =>
                obj.CreateCategoryAsync(
                    It.IsAny<CreateCategoryParams>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new Result<CreateCategoryResult, CategoryServiceResultCode>(categoryResult)
            );
    }
}
