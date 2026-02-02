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

    public async Task<Amount?> GetAmount(Func<Amount, bool> condition)
    {
        return await _context.Amounts.FirstOrDefaultAsync(amount => condition(amount));
    }

    public async Task<IEnumerable<Amount>> GetAmounts(Func<Amount, bool> condition)
    {
        return await _context.Amounts.Where(amount => condition(amount)).ToListAsync();
    }
}
