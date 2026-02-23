using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.AmountManagement;
using FinBookeAPI.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FinBookeAPI.Services.AmountManagement;

public interface IAmountManagementService
{
    /// <summary>
    /// This method adds an amount object to the database.
    /// </summary>
    /// <param name="amount">
    /// The amount object that should be added.
    /// </param>
    /// <returns>
    /// The amount object which has been added to the database.
    /// </returns>
    /// <exception cref="ValidationException">
    /// If a specific amount property does not fulfill the
    /// requirements.
    /// </exception>
    /// <exception cref="IdGenerationException">
    /// If a unique id could not be generated.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If read / write operations of the database have been canceled.
    /// </exception>
    /// <exception cref="DbUpdateException">
    /// If the amount collection could not be updated.
    /// </exception>
    /// <exception cref="DbUpdateConcurrencyException">
    /// If the amount collection could not be updated due to concurrency issues.
    /// </exception>
    public Task<Amount> CreateAmount(Amount amount);
}
