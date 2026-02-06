using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Tests.SecurityUtility;

public partial class SecurityUtilityUnitTests
{
    [Fact]
    public async Task Should_ReturnProvidedId_WhenIdIsUnique()
    {
        var id = Guid.NewGuid();
        static Task<bool> func(Guid id) => Task.FromResult(false);

        var result = await _service.GenerateUniqueId(id, func);

        Assert.Equal(id, result);
    }

    [Fact]
    public async Task Should_ReturnNewId_WhenProvidedIdIsNotUnique()
    {
        var id = Guid.Empty;
        static Task<bool> func(Guid id)
        {
            if (id == Guid.Empty)
                return Task.FromResult(true);
            return Task.FromResult(false);
        }

        var result = await _service.GenerateUniqueId(id, func);

        Assert.NotEqual(id, result);
    }

    [Fact]
    public async Task Should_FailGeneratingId_WhenAllGeneratedIdsAreAlreadyAssigned()
    {
        var id = Guid.NewGuid();
        static Task<bool> func(Guid id) => Task.FromResult(true);

        await Assert.ThrowsAsync<IdGenerationException>(() => _service.GenerateUniqueId(id, func));
    }
}
