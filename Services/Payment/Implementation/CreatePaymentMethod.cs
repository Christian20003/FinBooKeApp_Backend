using FinBookeAPI.AppConfig.Documentation;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> CreatePaymentMethod(PaymentMethod method)
    {
        _logger.LogDebug("Create new payment method {method}", method.ToString());
        VerifyPaymentMethod(method);
        var entity = await _collection.GetPaymentMethod(elem => elem.Id == method.Id);
        if (entity is not null)
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.PaymentMethodInsertFailed,
                new DuplicateEntityException("Payment method does already exist")
            );
        entity = method.Copy();
        _collection.AddPaymentMethod(entity);
        await _collection.SaveChanges();
        _logger.LogInformation(
            LogEvents.PaymentMethodInsertSuccess,
            "Payment method has been created successfully"
        );
        return method;
    }
}
