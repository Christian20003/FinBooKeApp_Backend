using FinBookeAPI.Models.AmountManagement;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.AmountManagement;

public partial class AmountManagementService : IAmountManagementService
{
    public async Task<Amount> CreateAmount(Amount amount)
    {
        LogCreateAmount(amount.Id);
        await VerifyAmount(amount);
        amount.Id = await _security.GenerateUniqueId(amount.Id, _collection.ExistsAmountId);
        _collection.AddAmount(amount);
        await _collection.SaveChanges();
        LogAmountCreated(amount.Id);
        return amount.Copy();
    }

    [LoggerMessage(
        EventId = LogEvents.AmountCreate,
        Level = LogLevel.Information,
        Message = "Amount: Create amount - AmountId: {Id}"
    )]
    private partial void LogCreateAmount(Guid id);

    [LoggerMessage(
        EventId = LogEvents.AmountCreateSuccess,
        Level = LogLevel.Information,
        Message = "Amount: Amount created - AmountId: {Id}"
    )]
    private partial void LogAmountCreated(Guid id);
}
