using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods(Guid userId)
    {
        LogGetPaymentMethods(userId);
        var entities = await _collection.GetPaymentMethods(elem => elem.UserId == userId);
        LogGetPaymentMethodsSuccess(userId);
        return entities.Select(elem => elem.Copy());
    }

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodRead,
        Level = LogLevel.Information,
        Message = "Payment: Get payment methods from {UserId}"
    )]
    private partial void LogGetPaymentMethods(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.PaymentMethodReadSuccess,
        Level = LogLevel.Information,
        Message = "Payment: Payment methods have been read successfully - {UserId}"
    )]
    private partial void LogGetPaymentMethodsSuccess(Guid userId);
}
