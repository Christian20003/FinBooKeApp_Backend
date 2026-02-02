using FinBookeAPI.AppConfig.Database;
using FinBookeAPI.Models.Payment;
using Microsoft.EntityFrameworkCore;

namespace FinBookeAPI.Collections.PaymentMethodCollection;

public class PaymentMethodCollection(DataDbContext context)
    : DataCollection(context),
        IPaymentMethodCollection
{
    private readonly DataDbContext _context = context;

    public void AddPaymentMethod(PaymentMethod method)
    {
        _context.PaymentMethods.Add(method);
    }

    public void UpdatePaymentMethod(PaymentMethod method)
    {
        _context.PaymentMethods.Update(method);
    }

    public void RemovePaymentMethod(PaymentMethod method)
    {
        _context.PaymentMethods.Remove(method);
    }

    public async Task<PaymentMethod?> GetPaymentMethod(Func<PaymentMethod, bool> condition)
    {
        return await _context.PaymentMethods.FirstOrDefaultAsync(elem => condition(elem));
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods(
        Func<PaymentMethod, bool> condition
    )
    {
        return await _context.PaymentMethods.Where(elem => condition(elem)).ToListAsync();
    }
}
