using FinBookeAPI.Models.Exceptions;
using Newtonsoft.Json;

namespace FinBookeAPI.Tests.Payment;

public partial class PaymentMethodServiceUnitTests
{
    [Fact]
    public async Task Should_FailRemovingPaymentMethod_WhenPaymentMethodDoesNotExist()
    {
        var pm = _database.First();

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _paymentMethodService.RemovePaymentMethod(_paymentMethod.Id, pm.UserId)
        );
    }

    [Fact]
    public async Task Should_FailRemovingPaymentMethod_WhenPaymentMethodNotAccessible()
    {
        var pm = _database.First();

        await Assert.ThrowsAsync<AuthorizationException>(
            () => _paymentMethodService.RemovePaymentMethod(pm.Id, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_RemovePaymentMethodFromDatabase_WhenPaymentMethodDataIsValid()
    {
        var pm = _database.First();

        var result = await _paymentMethodService.RemovePaymentMethod(pm.Id, pm.UserId);

        Assert.DoesNotContain(_database, elem => elem.Id == pm.Id);
    }

    [Fact]
    public async Task Should_ReturnRemovedPaymentMethod_WhenPaymentMethodDataIsValid()
    {
        var pm = _database.First();

        var result = await _paymentMethodService.RemovePaymentMethod(pm.Id, pm.UserId);

        var expected = JsonConvert.SerializeObject(pm);
        var actual = JsonConvert.SerializeObject(result);

        Assert.Equal(expected, actual);
    }
}
