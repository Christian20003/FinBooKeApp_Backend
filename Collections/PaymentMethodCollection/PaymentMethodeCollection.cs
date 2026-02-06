using System.Linq.Expressions;
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

    public async Task<PaymentMethod?> GetPaymentMethod(
        Expression<Func<PaymentMethod, bool>> condition
    )
    {
        return await _context.PaymentMethods.FirstOrDefaultAsync(condition);
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods(
        Expression<Func<PaymentMethod, bool>> condition
    )
    {
        return await _context.PaymentMethods.Where(condition).ToListAsync();
    }

    public async Task<bool> ExistsPaymentMethodId(Guid id)
    {
        return await _context.PaymentMethods.AnyAsync(method => method.Id == id);
    }

    public async Task<bool> ExistsPaymentInstanceId(Guid id)
    {
        return await _context.PaymentMethods.AnyAsync(method =>
            method.Instances.Any(instance => instance.Id == id)
        );
    }
}
