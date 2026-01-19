using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    public async Task<PaymentMethod> UpdatePaymentMethod(PaymentMethod method)
    {
        _logger.LogDebug("Update existing payment method {method}", method.ToString());
        VerifyPaymentMethod(method);
        var entity = await VerifyPaymentMethodAccess(method.Id, method.UserId);
        if (entity.Type != method.Type)
            entity.Type = new string(method.Type);

        var toAdd = method
            .Instances.Where(elem => !entity.Instances.Contains(elem))
            .Select(elem => elem.Copy());
        var toRemove = entity.Instances.Where(elem => !method.Instances.Contains(elem));
        if (toAdd.Any())
            entity.Instances = [.. entity.Instances, .. toAdd];
        if (toRemove.Any())
            entity.Instances = [.. entity.Instances.Where(elem => !toRemove.Contains(elem))];

        _collection.UpdatePaymentMethod(entity);
        await _collection.SaveChanges();
        _logger.LogInformation(
            LogEvents.PaymentMethodUpdateSuccess,
            "Payment method has been updated successfully"
        );
        return method;
    }
}
