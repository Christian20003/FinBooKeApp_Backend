using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> RemovePaymentMethod(Guid methodId, Guid userId)
    {
        _logger.LogDebug("Remove payment method {id}", methodId);
        var entity = await VerifyPaymentMethodAccess(methodId, userId);
        _collection.RemovePaymentMethod(entity);
        await _collection.SaveChanges();
        _logger.LogInformation(
            LogEvents.PaymentMethodDeleteSuccess,
            "Payment method has been removed successfully"
        );
        return entity.Copy();
    }
}
