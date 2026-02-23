using FinBookeAPI.Models.AmountManagement;

namespace FinBookeAPI.Tests.Records
{
    public static class AmountRecord
    {
        public static Amount GetTestAmount()
        {
            return new Amount
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PaymentTypeId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Value = 100.0m,
                Type = AmountType.EXPENSE,
                Comment = "Test Comment",
                ReceiptFile = "receipt.pdf",
                BankStatementFile = "statement.pdf",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
        }

        public static List<Amount> GetTestAmounts()
        {
            return
            [
                new Amount
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    PaymentTypeId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(),
                    Value = 100.0m,
                    Type = AmountType.EXPENSE,
                    Comment = "Test Comment 1",
                    ReceiptFile = "receipt1.pdf",
                    BankStatementFile = "statement1.pdf",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                },
                new Amount
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    PaymentTypeId = Guid.NewGuid(),
                    CategoryId = Guid.NewGuid(),
                    Value = 200.0m,
                    Type = AmountType.INCOME,
                    Comment = "Test Comment 2",
                    ReceiptFile = "receipt2.pdf",
                    BankStatementFile = "statement2.pdf",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                },
            ];
        }
    }
}
