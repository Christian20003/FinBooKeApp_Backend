using FinBookeAPI.Collections.PaymentMethodCollection;
using FinBookeAPI.Models.Payment;
using FinBookeAPI.Services.Payment;
using FinBookeAPI.Tests.Mocks.Collections;
using FinBookeAPI.Tests.Records;
using Moq;

namespace FinBookeAPI.Tests.Payment;

public partial class PaymentMethodServiceUnitTests
{
    private readonly Mock<IPaymentMethodCollection> _paymentMethodCollection;
    private readonly PaymentMethodService _paymentMethodService;
    private readonly PaymentMethod _paymentMethod;
    private readonly List<PaymentMethod> _database;

    public PaymentMethodServiceUnitTests()
    {
        _paymentMethod = PaymentMethodRecord.GetObject();
        _database = PaymentMethodRecord.GetObjects();

        var logger = new Mock<ILogger<PaymentMethodService>>();
        _paymentMethodCollection = MockPaymentMethodCollection.GetMock(_database);
        _paymentMethodService = new PaymentMethodService(
            _paymentMethodCollection.Object,
            logger.Object
        );
    }
}
