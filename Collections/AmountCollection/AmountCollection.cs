using System.Linq.Expressions;
using FinBookeAPI.AppConfig.Database;
using FinBookeAPI.Models.AmountManagement;
using Microsoft.EntityFrameworkCore;

namespace FinBookeAPI.Collections.AmountCollection;

public class AmountCollection(DataDbContext context) : DataCollection(context), IAmountCollection
{
    private readonly DataDbContext _context = context;

    public void AddAmount(Amount amount)
    {
        _context.Amounts.Add(amount);
    }

    public void UpdateAmount(Amount amount)
    {
        _context.Amounts.Update(amount);
    }

    public void RemoveAmount(Amount amount)
    {
        _context.Amounts.Remove(amount);
    }

    public async Task<Amount?> GetAmount(Expression<Func<Amount, bool>> condition)
    {
        return await _context.Amounts.FirstOrDefaultAsync(condition);
    }

    public async Task<IEnumerable<Amount>> GetAmounts(Expression<Func<Amount, bool>> condition)
    {
        return await _context.Amounts.Where(condition).ToListAsync();
    }

    public async Task<bool> ExistsAmountId(Guid id)
    {
        return await _context.Amounts.AnyAsync(amount => amount.Id == id);
    }
}
