using System.Linq.Expressions;
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
            .Setup(obj => obj.GetPaymentMethod(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<PaymentMethod, bool>> predicate) =>
                {
                    var func = predicate.Compile();
                    return data.FirstOrDefault(elem => func(elem));
                }
            );
        result
            .Setup(obj => obj.GetPaymentMethods(It.IsAny<Expression<Func<PaymentMethod, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<PaymentMethod, bool>> predicate) =>
                {
                    var func = predicate.Compile();
                    return data.Where(elem => func(elem));
                }
            );
        result
            .Setup(obj => obj.IsPaymentMethodIdUnique(It.IsAny<Guid>()))
            .ReturnsAsync(
                (Guid id) =>
                {
                    return data.Any(elem => elem.Id == id);
                }
            );
        result
            .Setup(obj => obj.IsPaymentInstanceIdUnique(It.IsAny<Guid>()))
            .ReturnsAsync(
                (Guid id) =>
                {
                    return data.Any(elem => elem.Instances.Any(instance => instance.Id == id));
                }
            );
        return result;
    }
}
