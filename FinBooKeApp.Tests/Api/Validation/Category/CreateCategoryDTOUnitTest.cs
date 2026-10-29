using System.ComponentModel.DataAnnotations;
using FinBooKeApp.Api.Controllers.Categories.Models;
using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Tests.Api.Validation.Category;

public class CategoryRequestDTOUnitTest
{
    [Fact]
    public async Task CreateCategory_WhenCategoryNameIsMissing_FailValidation()
    {
        var dto = new CategoryRequestDTO { Name = null!, Color = "#ffffff" };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.Name))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryColorIsMissing_FailValidation()
    {
        var dto = new CategoryRequestDTO { Name = "Name", Color = null! };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.Color))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryColorIsInvalid_FailValidation()
    {
        var dto = new CategoryRequestDTO { Name = "Name", Color = "ffffff" };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.Color))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryChildIdIsInvalid_FailValidation()
    {
        var dto = new CategoryRequestDTO
        {
            Name = "Name",
            Color = "#ffffff",
            Children = [Guid.Empty],
        };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.Children))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryLimitAmountIsNegative_FailValidation()
    {
        var dto = new CategoryRequestDTO
        {
            Name = "Name",
            Color = "#ffffff",
            LimitAmount = -13.0m,
            LimitStart = DateTime.Now,
            LimitMonthlyPeriod = TimePeriod.ONE_MONTH,
        };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.LimitAmount))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryLimitAmountIsSetAndOtherLimitFieldsMissing_FailValidation()
    {
        var dto = new CategoryRequestDTO
        {
            Name = "Name",
            Color = "#ffffff",
            LimitAmount = 3.0m,
        };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.LimitStart))
        );
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.LimitMonthlyPeriod))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryLimitMonthlyPeriodIsSetAndOtherLimitFieldsMissing_FailValidation()
    {
        var dto = new CategoryRequestDTO
        {
            Name = "Name",
            Color = "#ffffff",
            LimitMonthlyPeriod = TimePeriod.ONE_MONTH,
        };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.LimitAmount))
        );
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.LimitStart))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryLimitStartIsSetAndOtherLimitFieldsMissing_FailValidation()
    {
        var dto = new CategoryRequestDTO
        {
            Name = "Name",
            Color = "#ffffff",
            LimitStart = DateTime.Now,
        };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.LimitAmount))
        );
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CategoryRequestDTO.LimitMonthlyPeriod))
        );
    }
}
