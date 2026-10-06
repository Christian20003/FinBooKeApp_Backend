namespace FinBookeAPI.Tests.CategoryType;

public partial class CategoryServiceUnitTests
{
    [Fact]
    public void Should_ReturnNestedCategories()
    {
        var result = _service.NestCategories(_database);
        var parent = result.FirstOrDefault(elem => elem.Children.Any());

        Assert.Equal(2, result.Count());
        Assert.Equal(2, parent!.Children.Count());
    }
}
