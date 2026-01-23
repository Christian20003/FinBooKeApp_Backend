using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> CreatePaymentMethod(PaymentMethod method)
    {
        LogCreatePaymentMethod(method);
        VerifyPaymentMethod(method);
        var entity = await _collection.GetPaymentMethod(elem => elem.Id == method.Id);
        if (entity is not null)
        {
            LogExistingPaymentMethod(method);
            throw new DuplicateEntityException("Payment method does already exist");
        }
        entity = method.Copy();
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

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodDuplicate,
        Level = LogLevel.Error,
        Message = "Payment: Payment method already exists - {Method}"
    )]
    private partial void LogExistingPaymentMethod(PaymentMethod method);
}
