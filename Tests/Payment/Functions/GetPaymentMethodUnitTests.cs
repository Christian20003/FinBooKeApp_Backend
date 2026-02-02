using FinBookeAPI.Models.Exceptions;
using Newtonsoft.Json;

namespace FinBookeAPI.Tests.Payment;

public partial class PaymentMethodServiceUnitTests
{
    [Fact]
    public async Task Should_FailGettingPaymentMethod_WhenMethodIsNotAccessible()
    {
        var pm = _database.First();

        await Assert.ThrowsAsync<AuthorizationException>(
            () => _paymentMethodService.GetPaymentMethod(pm.Id, _paymentMethod.UserId)
        );
    }

    [Fact]
    public async Task Should_FailGettingPaymentMethod_WhenMethodDoesNotExist()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _paymentMethodService.GetPaymentMethod(Guid.NewGuid(), _paymentMethod.UserId)
        );
    }

    [Fact]
    public async Task Should_ReturnCopyOfRequestedPaymentMethod()
    {
        var pm = _database.First();
        var result = await _paymentMethodService.GetPaymentMethod(pm.Id, pm.UserId);

        Assert.NotSame(pm, result);
        foreach (var instance in result.Instances)
        {
            var pi = pm.Instances.First(elem => elem.Id == instance.Id);
            Assert.NotSame(pi, instance);
        }
    }

    [Fact]
    public async Task Should_ReturnRequestedPaymentMethod()
    {
        var pm = _database.First();
        var result = await _paymentMethodService.GetPaymentMethod(pm.Id, pm.UserId);

        var expected = JsonConvert.SerializeObject(pm);
        var actual = JsonConvert.SerializeObject(result);

        Assert.Equal(expected, actual);
    }
}
