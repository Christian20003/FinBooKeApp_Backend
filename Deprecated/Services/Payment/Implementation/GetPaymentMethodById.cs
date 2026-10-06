using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> GetPaymentMethodById(Guid id, Guid userId)
    {
        LogGetPaymentMethodById(id, userId);
        var entity = await _collection.GetPaymentMethod(elem =>
            elem.Instances.Any(instance => instance.Id == id)
        );
        if (entity is null)
        {
            entity = await VerifyPaymentMethodAccess(id, userId);
        }
        else
        {
            entity = await VerifyPaymentMethodAccess(entity.Id, userId);
        }
        LogGetPaymentMethodByIdSuccess(entity);
        return entity.Copy();
    }

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodRead,
        Level = LogLevel.Information,
        Message = "Payment: Get payment method by id - {Id} from {UserId}"
    )]
    private partial void LogGetPaymentMethodById(Guid id, Guid userId);

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodReadSuccess,
        Level = LogLevel.Information,
        Message = "Payment: Payment method has been read successfully - {Method}"
    )]
    private partial void LogGetPaymentMethodByIdSuccess(PaymentMethod method);
}
