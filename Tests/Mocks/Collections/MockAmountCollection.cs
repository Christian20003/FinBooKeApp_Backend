using System.Linq.Expressions;
using FinBookeAPI.Collections.AmountCollection;
using FinBookeAPI.Models.AmountManagement;
using Moq;

namespace FinBookeAPI.Tests.Mocks.Collections
{
    public static class MockAmountCollection
    {
        public static Mock<IAmountCollection> GetMock(List<Amount> database)
        {
            var mock = new Mock<IAmountCollection>();

            mock.Setup(m => m.AddAmount(It.IsAny<Amount>())).Callback<Amount>(database.Add);

            mock.Setup(m => m.UpdateAmount(It.IsAny<Amount>()))
                .Callback<Amount>(amount =>
                {
                    var existingAmount = database.FirstOrDefault(a => a.Id == amount.Id);
                    if (existingAmount != null)
                    {
                        var index = database.IndexOf(existingAmount);
                        database[index] = amount;
                    }
                });

            mock.Setup(m => m.RemoveAmount(It.IsAny<Amount>()))
                .Callback<Amount>(amount =>
                {
                    var existingAmount = database.FirstOrDefault(a => a.Id == amount.Id);
                    if (existingAmount != null)
                    {
                        database.Remove(existingAmount);
                    }
                });

            mock.Setup(m => m.GetAmount(It.IsAny<Expression<Func<Amount, bool>>>()))
                .ReturnsAsync(
                    (Expression<Func<Amount, bool>> predicate) =>
                    {
                        var func = predicate.Compile();
                        return database.FirstOrDefault(func);
                    }
                );

            mock.Setup(m => m.GetAmounts(It.IsAny<Expression<Func<Amount, bool>>>()))
                .ReturnsAsync(
                    (Expression<Func<Amount, bool>> predicate) =>
                    {
                        var func = predicate.Compile();
                        return [.. database.Where(func)];
                    }
                );

            mock.Setup(m => m.ExistsAmountId(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => database.Any(a => a.Id == id));

            mock.Setup(m => m.SaveChanges()).Returns(Task.CompletedTask);

            return mock;
        }
    }
}
