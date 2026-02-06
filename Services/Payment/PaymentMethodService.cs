using FinBookeAPI.Collections.PaymentMethodCollection;
using FinBookeAPI.Services.SecurityUtility;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService(
    ISecurityUtilityService security,
    IPaymentMethodCollection collection,
    ILogger<PaymentMethodService> logger
) : IPaymentMethodService
{
    private readonly IPaymentMethodCollection _collection = collection;

    private readonly ISecurityUtilityService _security = security;

    private readonly ILogger<PaymentMethodService> _logger = logger;
}
