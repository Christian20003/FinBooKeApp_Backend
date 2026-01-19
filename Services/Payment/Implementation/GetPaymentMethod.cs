using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> GetPaymentMethod(Guid methodId, Guid userId)
    {
        _logger.LogDebug("Get payment method {id}", methodId);
        var entity = await VerifyPaymentMethodAccess(methodId, userId);
        _logger.LogInformation(
            LogEvents.PaymentMethodReadSuccess,
            "Payment method has been read successfully"
        );
        return entity.Copy();
    }
}
