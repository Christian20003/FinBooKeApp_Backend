namespace FinBookeAPI.Tests.Payment;

public partial class PaymentMethodServiceUnitTests
{
    [Fact]
    public async Task Should_ReturnCopiesOfRequestedPaymentMethods()
    {
        var pm = _database.First();
        var result = await _paymentMethodService.GetPaymentMethods(pm.UserId);

        var storedPms = _database.Where(elem => elem.UserId == pm.UserId);
        var storedPis = storedPms.SelectMany(elem => elem.Instances);

        foreach (var method in result)
        {
            Assert.NotSame(method, storedPms.First(elem => elem.Id == method.Id));
        }

        foreach (var instance in result.SelectMany(elem => elem.Instances))
        {
            Assert.NotSame(instance, storedPis.First(elem => elem.Id == instance.Id));
        }
    }

    [Fact]
    public async Task Should_ReturnRequestedPaymentMethods()
    {
        var pm = _database.First();
        var result = await _paymentMethodService.GetPaymentMethods(pm.UserId);

        Assert.Equal(_database.Count(elem => elem.UserId == pm.UserId), result.Count());
    }
}
