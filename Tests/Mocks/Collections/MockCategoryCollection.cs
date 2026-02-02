using FinBookeAPI.Collections.CategoryCollection;
using FinBookeAPI.Models.CategoryType;
using Moq;

namespace FinBookeAPI.Tests.Mocks.Collections;

public static class MockCategoryCollection
{
    public static Mock<ICategoryCollection> GetMock(List<Category> data)
    {
        var result = new Mock<ICategoryCollection>();
        result.Setup(obj => obj.CreateCategory(It.IsAny<Category>())).Callback<Category>(data.Add);
        result
            .Setup(obj => obj.UpdateCategory(It.IsAny<Category>()))
            .Callback<Category>(input =>
            {
                var idx = data.FindIndex(elem => elem.Id == input.Id);
                if (idx != -1)
                    data[idx] = input;
            });
        result
            .Setup(obj => obj.DeleteCategory(It.IsAny<Category>()))
            .Callback<Category>(input =>
            {
                data.Remove(input);
            });
        result
            .Setup(obj => obj.GetCategory(It.IsAny<Func<Category, bool>>()))
            .ReturnsAsync(
                (Func<Category, bool> condition) =>
                {
                    return data.FirstOrDefault(elem => condition(elem));
                }
            );
        result
            .Setup(obj => obj.GetCategories(It.IsAny<Func<Category, bool>>()))
            .ReturnsAsync(
                (Func<Category, bool> condition) =>
                {
                    return data.Where(elem => condition(elem));
                }
            );
        return result;
    }
}
