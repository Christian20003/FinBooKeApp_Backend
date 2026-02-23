using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.AmountManagement;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.AmountManagement;

//TODO: Certain properties are currently not checked if they point to
// a correct entity.
public partial class AmountManagementService : IAmountManagementService
{
    /// <summary>
    /// This method verifies every property of a given amount object
    /// to fulfill all defined requirements.
    /// </summary>
    /// <param name="amount">
    /// The amount object that should be validated.
    /// </param>
    /// <exception cref="ValidationException">
    /// If a specific requirement is not fulfilled.
    /// </exception>
    private async Task VerifyAmount(Amount amount)
    {
        LogVerifyAmount(amount.Id);
        var validator = new ValidationContext(amount);
        Validator.ValidateObject(amount, validator, true);
    }

    /// <summary>
    /// This method verifies an existing amount object. Thereby
    /// all properties will be checked if each fulfill defined
    /// requirements.
    /// </summary>
    /// <param name="amount">
    /// The existing amount object that should be verified.
    /// </param>
    /// <returns>
    /// The amount object from the database.
    /// </returns>
    /// <exception cref="ValidationException">
    /// If a specific property requirement is not fulfilled.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the amount object could not be found in the database.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the user does not have access to the amount object
    /// in the database.
    /// </exception>
    private async Task<Amount> VerifyExistingAmount(Amount amount)
    {
        LogVerifyExistingAmount(amount.Id);
        await VerifyAmount(amount);
        var entity = await _collection.GetAmount(entity => entity.Id == amount.Id);
        if (entity is null)
        {
            LogAmountNotFound(amount.Id);
            throw new EntityNotFoundException("Amount object does not exist");
        }
        if (entity.UserId != amount.UserId)
        {
            LogAmountNotAccessible(amount.Id, amount.UserId);
            throw new AuthorizationException("Amount object is not accessible");
        }
        return entity;
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Amount: Verify amount - AmountId: {Id}")]
    private partial void LogVerifyAmount(Guid id);

    [LoggerMessage(LogLevel.Trace, Message = "Amount: Verify existing amount - AmountId: {Id}")]
    private partial void LogVerifyExistingAmount(Guid id);

    [LoggerMessage(
        EventId = LogEvents.AmountNotFound,
        Level = LogLevel.Error,
        Message = "Amount: Amount not found - AmountId: {Id}"
    )]
    private partial void LogAmountNotFound(Guid id);

    [LoggerMessage(
        EventId = LogEvents.AmountNotAccessible,
        Level = LogLevel.Error,
        Message = "Amount: Amount not accessible - AmountId: {Id}, UserId: {UserId}"
    )]
    private partial void LogAmountNotAccessible(Guid id, Guid userId);
}
