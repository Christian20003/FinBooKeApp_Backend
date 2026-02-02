using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Tests.Records;

public static class PaymentMethodRecord
{
    public static PaymentMethod GetObject()
    {
        return new PaymentMethod
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Type = "Visa",
            Instances =
            [
                new PaymentInstance
                {
                    Id = Guid.NewGuid(),
                    Name = "Debit Card",
                    Description = "Data: 12345",
                },
            ],
        };
    }

    public static List<PaymentMethod> GetObjects()
    {
        var userId = Guid.NewGuid();
        return
        [
            new PaymentMethod
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = "Mastercard",
                Instances = [new PaymentInstance { Id = Guid.NewGuid(), Name = "Credit Card" }],
            },
            new PaymentMethod
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = "PayPal",
            },
        ];
    }
}
