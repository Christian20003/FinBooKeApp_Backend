using System.Linq.Expressions;
using FinBookeAPI.Collections.CategoryCollection;
using FinBookeAPI.Models.Category;
using Moq;

namespace FinBookeAPI.Tests.Mocks.Collections;

public static class MockCategoryCollection
{
    public static Mock<ICategoryCollection> GetMock(List<CategoryTag> data)
    {
        var result = new Mock<ICategoryCollection>();
        result
            .Setup(obj => obj.CreateCategory(It.IsAny<CategoryTag>()))
            .Callback<CategoryTag>(data.Add);
        result
            .Setup(obj => obj.UpdateCategory(It.IsAny<CategoryTag>()))
            .Callback<CategoryTag>(input =>
            {
                var idx = data.FindIndex(elem => elem.Id == input.Id);
                if (idx != -1)
                    data[idx] = input;
            });
        result
            .Setup(obj => obj.DeleteCategory(It.IsAny<CategoryTag>()))
            .Callback<CategoryTag>(input =>
            {
                data.Remove(input);
            });
        result
            .Setup(obj => obj.GetCategory(It.IsAny<Expression<Func<CategoryTag, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<CategoryTag, bool>> condition) =>
                {
                    var func = condition.Compile();
                    return data.FirstOrDefault(elem => func(elem));
                }
            );
        result
            .Setup(obj => obj.GetCategories(It.IsAny<Expression<Func<CategoryTag, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<CategoryTag, bool>> condition) =>
                {
                    var func = condition.Compile();
                    return data.Where(elem => func(elem));
                }
            );
        result
            .Setup(obj => obj.ExistsCategoryId(It.IsAny<Guid>()))
            .ReturnsAsync(
                (Guid id) =>
                {
                    return false;
                }
            );
        return result;
    }
}
