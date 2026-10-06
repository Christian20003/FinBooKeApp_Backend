using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> GetPaymentMethod(Guid methodId, Guid userId)
    {
        LogGetPaymentMethod(methodId, userId);
        var entity = await VerifyPaymentMethodAccess(methodId, userId);
        LogGetPaymentMethodSuccess(entity);
        return entity.Copy();
    }

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodRead,
        Level = LogLevel.Information,
        Message = "Payment: Get payment method - {Id} from {UserId}"
    )]
    private partial void LogGetPaymentMethod(Guid id, Guid userId);

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodReadSuccess,
        Level = LogLevel.Information,
        Message = "Payment: Payment method has been read successfully - {Method}"
    )]
    private partial void LogGetPaymentMethodSuccess(PaymentMethod method);
}
