using FinBookeAPI.AppConfig.Documentation;
using FinBookeAPI.Models.AmountManagement;
using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.AmountManagement;

public partial class AmountManagementService : IAmountManagementService
{
    private async Task<(Amount?, Category?)> VerifyAmount(Amount amount)
    {
        var amountDb = await VerifyUserId(amount.Id, amount.UserId);
        var categoryDb = await _category.GetCategory(amount.CategoryId, amount.UserId);
        VerifyValue(amount.Value);
        VerifyComment(amount.Comment);
        VerifyReceiptFile(amount.ReceiptFile, amount.UserId);
        VerifyBankStatementFile(amount.BankStatementFile, amount.UserId);
        return (amountDb, categoryDb);
    }

    /// <summary>
    /// This method verifies the amount id.
    /// </summary>
    /// <param name="amountId">
    /// The if of the amount object that should be validated.
    /// </param>
    /// <returns>
    /// The amount object from the database. If the database
    /// does not find an object with the provided id,
    /// <c>null</c> is returned.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the amount id is an empty Guid.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the search operation with the database has been
    /// canceled.
    /// </exception>
    private async Task<Amount?> VerifyId(Guid amountId)
    {
        if (Guid.Empty == amountId)
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.AmountManagementOperationFailed,
                new ArgumentException("Amount id is not a valid GUID", nameof(amountId))
            );
        return await _collection.GetAmount(amount => amount.Id == amountId);
    }

    /// <summary>
    /// This method verifies the user id of the amount object.
    /// </summary>
    /// <param name="amountId">
    /// The id of the amount object.
    /// </param>
    /// <param name="userId">
    /// The user id who wants to access the amount object.
    /// </param>
    /// <returns>
    /// The amount object from the database. If the database
    /// does not find an object with the provided id,
    /// <c>null</c> is returned.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If the amount id is an empty Guid.
    /// If the user id is an empty Guid.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the user does not have access to the amount object.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the search operation with the database has been
    /// canceled.
    /// </exception>
    private async Task<Amount?> VerifyUserId(Guid amountId, Guid userId)
    {
        var amount = await VerifyId(amountId);
        if (Guid.Empty == userId)
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.AmountManagementOperationFailed,
                new ArgumentException("User id is not a valid GUID", nameof(userId))
            );
        if (amount is null)
            return null;
        if (amount.UserId != userId)
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.AmountManagementOperationFailed,
                new AuthorizationException("Amount resource is not accessible")
            );
        return amount;
    }

    /// <summary>
    /// This method verifies the provided value of an amount
    /// object.
    /// </summary>
    /// <param name="value">
    /// The value that should be verified.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the provided amount value is equal or smaller than zero.
    /// </exception>
    private void VerifyValue(decimal value)
    {
        if (value <= 0)
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.AmountManagementOperationFailed,
                new ArgumentException("Amount value must be larger than zero", nameof(value))
            );
    }

    /// <summary>
    /// This method verifies the comment of a amount object.
    /// </summary>
    /// <param name="comment">
    /// The comment that should be verified.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the comment exceed the size limit of 300
    /// character.
    /// </exception>
    private void VerifyComment(string comment)
    {
        if (comment.Length > 300)
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.AmountManagementOperationFailed,
                new ArgumentException(
                    "Amount comment can only have up to 300 characters",
                    nameof(comment)
                )
            );
    }

    /// <summary>
    /// This method verfies the receipt file of an amount object.
    /// </summary>
    /// <param name="filename">
    /// The name of the receipt file.
    /// </param>
    /// <param name="userId">
    /// The user id who ownes the receipt file.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the receipt file does not exist.
    /// </exception>
    private void VerifyReceiptFile(string filename, Guid userId)
    {
        var path = Path.Combine(FileStorage.GetReceiptPath(userId), filename);
        if (string.IsNullOrEmpty(filename))
            return;
        if (!File.Exists(path))
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.AmountManagementOperationFailed,
                new ArgumentException("Amount receipt document does not exist", nameof(filename))
            );
    }

    /// <summary>
    /// This method verfies the bank statement file of an amount object.
    /// </summary>
    /// <param name="filename">
    /// The name of the bank statement file.
    /// </param>
    /// <param name="userId">
    /// The user id who ownes the bank statement file.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If the bank statement file does not exist.
    /// </exception>
    private void VerifyBankStatementFile(string filename, Guid userId)
    {
        var path = Path.Combine(FileStorage.GetReceiptPath(userId), filename);
        if (string.IsNullOrEmpty(filename))
            return;
        if (!File.Exists(path))
            Logging.ThrowAndLogWarning(
                _logger,
                LogEvents.AmountManagementOperationFailed,
                new ArgumentException(
                    "Amount bank statement document does not exist",
                    nameof(filename)
                )
            );
    }
}
