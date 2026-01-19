using FinBookeAPI.Collections.PaymentMethodCollection;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService(
    IPaymentMethodCollection collection,
    ILogger<PaymentMethodService> logger
) : IPaymentMethodService
{
    private readonly IPaymentMethodCollection _collection = collection;

    private readonly ILogger<PaymentMethodService> _logger = logger;
}
