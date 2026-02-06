using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Exceptions;
using FinBookeAPI.Models.Payment;
using Microsoft.EntityFrameworkCore;

namespace FinBookeAPI.Services.Payment;

public interface IPaymentMethodService
{
    /// <summary>
    /// This method inserts a payment method to the database.
    /// </summary>
    /// <param name="method">
    /// The payment method to insert.
    /// </param>
    /// <returns>
    /// The inserted payment method.
    /// </returns>
    /// <exception cref="IdGenerationException">
    /// If no unique id could be generated for a payment method or
    /// payment instance.
    /// </exception>
    /// <exception cref="ValidationException">
    /// If the payment method is invalid.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the database operation is canceled
    /// by the application.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the database operation fails.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// If the database operation fails due to a concurrency conflict.
    /// </exception>
    public Task<PaymentMethod> CreatePaymentMethod(PaymentMethod method);

    /// <summary>
    /// This method updates a payment method in the database.
    /// </summary>
    /// <param name="method">
    /// The payment method to update with updated properties.
    /// </param>
    /// <returns>
    /// The updated payment method.
    /// </returns>
    /// <exception cref="EntityNotFoundException">
    /// If the payment method does not exist.
    /// </exception>
    /// <exception cref="ValidationException">
    /// If the payment method is invalid.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the payment method is not accessible.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the database operation is canceled
    /// by the application.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the database operation fails.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// If the database operation fails due to a concurrency conflict.
    /// </exception>
    public Task<PaymentMethod> UpdatePaymentMethod(PaymentMethod method);

    /// <summary>
    /// This method removes a payment method from the database.
    /// </summary>
    /// <param name="methodId">
    /// The id of the payment method to remove.
    /// </param>
    /// <param name="userId">
    /// The id of the user who owns the payment method.
    /// </param>
    /// <returns>
    /// The removed payment method.
    /// </returns>
    /// <exception cref="EntityNotFoundException">
    /// If the payment method does not exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the payment method is not accessible.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the database operation is canceled
    /// by the application.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the database operation fails.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// If the database operation fails due to a concurrency conflict.
    /// </exception>
    public Task<PaymentMethod> RemovePaymentMethod(Guid methodId, Guid userId);

    /// <summary>
    /// This method gets a payment method from the database.
    /// </summary>
    /// <param name="methodId">
    /// The id of the payment method to get.
    /// </param>
    /// <param name="userId">
    /// The id of the user who owns the payment method.
    /// </param>
    /// <returns>
    /// The requested payment method.
    /// </returns>
    /// <exception cref="EntityNotFoundException">
    /// If the payment method does not exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the payment method is not accessible.
    /// </exception>
    public Task<PaymentMethod> GetPaymentMethod(Guid methodId, Guid userId);

    /// <summary>
    /// This method gets all payment methods of a user from the database.
    /// </summary>
    /// <param name="userId">
    /// The id of the user whose payment methods to get.
    /// </param>
    /// <returns>
    /// The requested payment methods.
    /// </returns>
    public Task<IEnumerable<PaymentMethod>> GetPaymentMethods(Guid userId);

    /// <summary>
    /// This method returns the payment method with the corresponding id
    /// or the payment method that includes a payment instance with the
    /// provided id.
    /// </summary>
    /// <param name="id">
    /// The id of the payment method or instance.
    /// </param>
    /// <param name="userId">
    /// The id of the user who wants to access to the payment method.
    /// </param>
    /// <returns>
    /// The payment method with the corresponding id or payment instance
    /// id.
    /// </returns>
    /// <exception cref="EntityNotFoundException">
    /// If the payment method or instances does not exist.
    /// </exception>
    /// <exception cref="AuthorizationException">
    /// If the payment method is not accessible.
    /// </exception>
    public Task<PaymentMethod> GetPaymentMethodById(Guid id, Guid userId);
}
