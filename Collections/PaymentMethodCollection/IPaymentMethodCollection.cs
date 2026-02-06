using System.Linq.Expressions;
using FinBookeAPI.Models.Payment;

namespace FinBookeAPI.Collections.PaymentMethodCollection;

public interface IPaymentMethodCollection : IDataCollection
{
    /// <summary>
    /// This method initializes tracking of the provided instance for insertion.
    /// </summary>
    /// <param name="method">
    /// The object that should be tracked.
    /// </param>
    ///
    public void AddPaymentMethod(PaymentMethod method);

    /// <summary>
    /// This method initializes tracking of the provided instance for updating.
    /// </summary>
    /// <param name="method">
    /// The object that should be tracked.
    /// </param>
    public void UpdatePaymentMethod(PaymentMethod method);

    /// <summary>
    /// This method initializes tracking of the provided instance for removing.
    /// </summary>
    /// <param name="method">
    /// The object that should be tracked.
    /// </param>
    public void RemovePaymentMethod(PaymentMethod method);

    /// <summary>
    /// This method returns the first instance that fulfills the
    /// specified requirements. If there is not any instance found,
    /// <c>null</c> will be returned.
    /// </summary>
    /// <param name="condition">
    /// A function that defines all requirements that the returning
    /// instance must fulfill.
    /// </param>
    /// <returns>
    /// The instance that fulfill the requirements, otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If an operation could not be executed at the application level
    /// and has been canceled.
    /// </exception>
    public Task<PaymentMethod?> GetPaymentMethod(Expression<Func<PaymentMethod, bool>> condition);

    /// <summary>
    /// This method returns a list of instances that fulfill the
    /// specified requirements. If there is not any instance found,
    /// the list will be empty.
    /// </summary>
    /// <param name="condition">
    /// A function that defines all requirements that the returning
    /// instances must fulfill.
    /// </param>
    /// <returns>
    /// A list of instances that fullfil the requirements.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If an operation could not be executed at the application level
    /// and has been canceled.
    /// </exception>
    public Task<IEnumerable<PaymentMethod>> GetPaymentMethods(
        Expression<Func<PaymentMethod, bool>> condition
    );

    /// <summary>
    /// This method proofs if the provided id is already assigned
    /// to a payment method.
    /// </summary>
    /// <param name="id">
    /// The id which should be checked.
    /// </param>
    /// <returns>
    /// `true` if the id is already assigned, otherwise `false`.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If an operation could not be executed at the application level
    /// and has been canceled.
    /// </exception>
    public Task<bool> IsPaymentMethodIdUnique(Guid id);

    /// <summary>
    /// This method proofs if the provided id is already assigned
    /// to a payment instance.
    /// </summary>
    /// <param name="id">
    /// The id which should be checked.
    /// </param>
    /// <returns>
    /// `true` if the id is already assigned, otherwise `false`.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If an operation could not be executed at the application level
    /// and has been canceled.
    /// </exception>
    public Task<bool> IsPaymentInstanceIdUnique(Guid id);
}
