using System.ComponentModel;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> GetPaymentMethodById(Guid id, Guid userId)
    {
        _logger.LogDebug("Get payment method from id {id}", id);
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
        _logger.LogInformation(
            LogEvents.PaymentMethodReadSuccess,
            "Payment method has been read successfully"
        );
        return entity.Copy();
    }
}
