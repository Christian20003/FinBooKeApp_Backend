using FinBookeAPI.AppConfig.Documentation;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods(Guid userId)
    {
        _logger.LogDebug("Get payment methods of user {id}", userId);
        var entities = await _collection.GetPaymentMethods(elem => elem.UserId == userId);
        _logger.LogInformation(
            LogEvents.PaymentMethodReadSuccess,
            "Payment methods have been read successfully"
        );
        return entities.Select(elem => elem.Copy());
    }
}
