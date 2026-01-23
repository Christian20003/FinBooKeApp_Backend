using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> RemovePaymentMethod(Guid methodId, Guid userId)
    {
        LogRemovePaymentMethod(methodId, userId);
        var entity = await VerifyPaymentMethodAccess(methodId, userId);
        _collection.RemovePaymentMethod(entity);
        await _collection.SaveChanges();
        LogRemovePaymentMethodSuccess(entity);
        return entity.Copy();
    }

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodDelete,
        Level = LogLevel.Information,
        Message = "Payment: Remove payment method - {Id} from {UserId}"
    )]
    private partial void LogRemovePaymentMethod(Guid id, Guid userId);

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodDeleteSuccess,
        Level = LogLevel.Information,
        Message = "Payment: Payment method has been removed successfully - {Method}"
    )]
    private partial void LogRemovePaymentMethodSuccess(PaymentMethod method);
}
