using FinBookeAPI.Collections.PaymentMethodCollection;
using FinBookeAPI.Models.Payment;
using Moq;

namespace FinBookeAPI.Tests.Mocks.Collections;

public static class MockPaymentMethodCollection
{
    public static Mock<IPaymentMethodCollection> GetMock(List<PaymentMethod> data)
    {
        var result = new Mock<IPaymentMethodCollection>();
        result
            .Setup(obj => obj.AddPaymentMethod(It.IsAny<PaymentMethod>()))
            .Callback<PaymentMethod>(data.Add);
        result
            .Setup(obj => obj.UpdatePaymentMethod(It.IsAny<PaymentMethod>()))
            .Callback<PaymentMethod>(input =>
            {
                var idx = data.FindIndex(elem => elem.Id == input.Id);
                if (idx != -1)
                    data[idx] = input;
            });
        result
            .Setup(obj => obj.RemovePaymentMethod(It.IsAny<PaymentMethod>()))
            .Callback<PaymentMethod>(input =>
            {
                data.Remove(input);
            });
        result
            .Setup(obj => obj.GetPaymentMethod(It.IsAny<Func<PaymentMethod, bool>>()))
            .ReturnsAsync(
                (Func<PaymentMethod, bool> predicate) =>
                {
                    return data.FirstOrDefault(elem => predicate(elem));
                }
            );
        result
            .Setup(obj => obj.GetPaymentMethods(It.IsAny<Func<PaymentMethod, bool>>()))
            .ReturnsAsync(
                (Func<PaymentMethod, bool> predicate) =>
                {
                    return data.Where(elem => predicate(elem));
                }
            );
        return result;
    }
}
