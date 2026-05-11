using FinBookeAPI.Models.Database.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// TODO: Deprecated

namespace FinBookeAPI.Models.Wrapper;

public class AccountManager(UserManager<UserAccount> userManager) : IAccountManager
{
    private readonly UserManager<UserAccount> _userManager = userManager;

    public async Task<IdentityResult> CreateUserAsync(UserAccount user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        Console.WriteLine(result);
        return result;
    }

    public IAsyncEnumerable<UserAccount> GetUsersAsync()
    {
        return _userManager.Users.AsAsyncEnumerable();
    }

    public async Task<IdentityResult> SetPasswordAsync(UserAccount user, string password)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return await _userManager.ResetPasswordAsync(user, token, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(UserAccount user)
    {
        return await _userManager.UpdateAsync(user);
    }
}
