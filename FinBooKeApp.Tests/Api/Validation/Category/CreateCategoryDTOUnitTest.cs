using System.ComponentModel.DataAnnotations;
using FinBooKeApp.Api.Controllers.Category.Models;
using FinBooKeApp.Data.Models;

namespace FinBooKeApp.Tests.Api.Validation.Category;

public class CreateCategoryDTOUnitTest
{
    [Fact]
    public async Task CreateCategory_WhenCategoryNameIsMissing_FailValidation()
    {
        var dto = new CreateCategoryDTO { Name = null!, Color = "#ffffff" };
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        Assert.False(isValid);
        Assert.Contains(results, item => item.MemberNames.Contains(nameof(CreateCategoryDTO.Name)));
    }

    [Fact]
    public void CreateCategory_WhenCategoryColorIsMissing_FailValidation()
    {
        var dto = new CreateCategoryDTO { Name = "Name", Color = null! };
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
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.Color))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryColorIsInvalid_FailValidation()
    {
        var dto = new CreateCategoryDTO { Name = "Name", Color = "ffffff" };
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
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.Color))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryChildIdIsInvalid_FailValidation()
    {
        var dto = new CreateCategoryDTO
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
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.Children))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryLimitAmountIsNegative_FailValidation()
    {
        var dto = new CreateCategoryDTO
        {
            Name = "Name",
            Color = "#ffffff",
            LimitAmount = -13.0m,
            LimitStart = DateTime.Now,
            LimitTimePeriod = TimePeriod.ONE_MONTH,
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
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.LimitAmount))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryLimitAmountIsSetAndOtherLimitFieldsMissing_FailValidation()
    {
        var dto = new CreateCategoryDTO
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
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.LimitStart))
        );
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.LimitTimePeriod))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryLimitTimePeriodIsSetAndOtherLimitFieldsMissing_FailValidation()
    {
        var dto = new CreateCategoryDTO
        {
            Name = "Name",
            Color = "#ffffff",
            LimitTimePeriod = TimePeriod.ONE_MONTH,
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
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.LimitAmount))
        );
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.LimitStart))
        );
    }

    [Fact]
    public void CreateCategory_WhenCategoryLimitStartIsSetAndOtherLimitFieldsMissing_FailValidation()
    {
        var dto = new CreateCategoryDTO
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
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.LimitAmount))
        );
        Assert.Contains(
            results,
            item => item.MemberNames.Contains(nameof(CreateCategoryDTO.LimitTimePeriod))
        );
    }
}
