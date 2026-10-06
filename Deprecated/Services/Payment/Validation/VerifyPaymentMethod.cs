using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Services.Payment;

public partial class PaymentMethodService : IPaymentMethodService
{
    /// <summary>
    /// This method verifies that each property requirement defined
    /// with attributes are fulfilled.
    /// </summary>
    /// <param name="method">
    /// The payment method that should be verified.
    /// </param>
    /// <exception cref="ValidationException">
    /// If the provided payment method does not fulfill a defined
    /// data annotation attribute.
    /// </exception>
    private void VerifyPaymentMethod(PaymentMethod method)
    {
        LogPaymentMethodValidation(method);
        var context = new ValidationContext(method);
        Validator.ValidateObject(method, context, true);

        foreach (var instance in method.Instances)
        {
            context = new ValidationContext(instance);
            Validator.ValidateObject(instance, context, true);
        }
    }

    /// <summary>
    /// This method verifies that the payment method is accessible by the user.
    /// </summary>
    /// <param name="methodId">
    /// The id of the payment method to verify.
    /// </param>
    /// <param name="userId">
    /// The id of the user who owns the payment method.
    /// </param>
    /// <returns>
    /// The payment method from the database.
    /// </returns>
    /// <exception cref="AuthorizationException">
    /// If the payment method is not accessible by the user.
    /// </exception>
    /// <exception cref="EntityNotFoundException">
    /// If the payment method does not exist.
    /// </exception>
    private async Task<PaymentMethod> VerifyPaymentMethodAccess(Guid methodId, Guid userId)
    {
        LogPaymentMethodAccessible(methodId);
        var entity = await _collection.GetPaymentMethod(elem => elem.Id == methodId);
        if (entity is null)
        {
            LogNotFoundPaymentMethod(methodId);
            throw new EntityNotFoundException("Payment method does not exist");
        }
        if (entity.UserId != userId)
        {
            LogNotAccessiblePaymentMethod(methodId);
            throw new AuthorizationException("Payment method is not accessible");
        }
        return entity;
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Payment: Verify payment method - {Method}")]
    private partial void LogPaymentMethodValidation(PaymentMethod method);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Payment: Verify if payment method is accessible - {Id}"
    )]
    private partial void LogPaymentMethodAccessible(Guid id);

    [LoggerMessage(
        EventId = LogEvents.PaymentNotFound,
        Level = LogLevel.Error,
        Message = "Payment: Payment method does not exist - {Id}"
    )]
    private partial void LogNotFoundPaymentMethod(Guid id);

    [LoggerMessage(
        EventId = LogEvents.PaymentNotAccessible,
        Level = LogLevel.Error,
        Message = "Payment: Payment method not accessible - {Id}"
    )]
    private partial void LogNotAccessiblePaymentMethod(Guid id);
}
