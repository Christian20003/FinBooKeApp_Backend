using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> CreatePaymentMethod(PaymentMethod method)
    {
        LogCreatePaymentMethod(method);

        VerifyPaymentMethod(method);
        method.Id = await _security.GenerateUniqueId(method.Id, _collection.ExistsPaymentMethodId);
        foreach (var instance in method.Instances)
        {
            instance.Id = await _security.GenerateUniqueId(
                instance.Id,
                _collection.ExistsPaymentInstanceId
            );
        }

        var entity = method.Copy();
        _collection.AddPaymentMethod(entity);
        await _collection.SaveChanges();
        LogPaymentMethodCreated(method);
        return method;
    }

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodCreate,
        Level = LogLevel.Information,
        Message = "Payment: Create payment method - {Method}"
    )]
    private partial void LogCreatePaymentMethod(PaymentMethod method);

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodCreateSuccess,
        Level = LogLevel.Information,
        Message = "Payment: Payment method created successfully - {Method}"
    )]
    private partial void LogPaymentMethodCreated(PaymentMethod method);
}
