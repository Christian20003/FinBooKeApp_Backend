using System.Linq.Expressions;
using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeAPI.Models.Database.Account;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FinBooKeAPI.Test.Mocks.Collections;

public static class MockAccountCollection
{
    public record InMemoryCollection
    {
        public List<UserAccount> Accounts { get; init; } = [];
        public Dictionary<string, string> RefreshTokens { get; init; } = [];
        public Dictionary<string, string> ResetTokens { get; init; } = [];
        public Dictionary<string, string> ChangeEmailTokens { get; init; } = [];
        public Dictionary<string, string> VerifyEmailTokens { get; init; } = [];
    }

    public static InMemoryCollection GetCollection()
    {
        var account = new UserAccount
        {
            Id = Guid.Empty.ToString(),
            UserName = "max",
            Email = "max.mustermann@gmail.com",
            EmailHash = "max.mustermann@gmail.com",
            ImagePath = "/path/to/image",
            PasswordHash = "password",
        };
        var collection = new InMemoryCollection { Accounts = [account] };
        return collection;
    }

    public static Mock<IAccountCollection> GetMock(InMemoryCollection collection)
    {
        var mock = new Mock<IAccountCollection>();

        mock.Setup(obj => obj.GetAccountAsync(It.IsAny<Expression<Func<UserAccount, bool>>>()))
            .ReturnsAsync(
                (Expression<Func<UserAccount, bool>> condition) =>
                {
                    return collection.Accounts.FirstOrDefault(condition.Compile());
                }
            );

        mock.Setup(obj =>
                obj.ChangeEmailAddressAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(
                (UserAccount account, string token, string email) =>
                {
                    var dbToken = collection.ChangeEmailTokens.GetValueOrDefault(email);
                    if (dbToken is null || dbToken != token)
                        return IdentityResult.Failed([]);
                    collection.Accounts.ForEach(
                        (item) =>
                        {
                            if (item.Id == account.Id)
                            {
                                item.Email = email;
                                item.EmailHash = email;
                            }
                        }
                    );
                    return IdentityResult.Success;
                }
            );

        mock.Setup(obj => obj.CreateAccountAsync(It.IsAny<UserAccount>(), It.IsAny<string>()))
            .ReturnsAsync(
                (UserAccount account, string password) =>
                {
                    if (password.Length == 0)
                        return IdentityResult.Failed([]);
                    collection.Accounts.Add(account);
                    return IdentityResult.Success;
                }
            );

        mock.Setup(obj => obj.DeleteAccountRefreshTokenAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(
                (UserAccount account) =>
                {
                    if (collection.RefreshTokens.Remove(account.Email!))
                        return IdentityResult.Success;
                    return IdentityResult.Failed([]);
                }
            );

        mock.Setup(obj => obj.GenerateChangeEmailToken(It.IsAny<UserAccount>(), It.IsAny<string>()))
            .ReturnsAsync(
                (UserAccount account, string email) =>
                {
                    var token = Guid.NewGuid().ToString();
                    collection.ChangeEmailTokens.Add(email, token);
                    return token;
                }
            );

        mock.Setup(obj => obj.GenerateEmailVerificationToken(It.IsAny<UserAccount>()))
            .ReturnsAsync(
                (UserAccount account) =>
                {
                    var token = Guid.NewGuid().ToString();
                    collection.VerifyEmailTokens.Add(account.Email!, token);
                    return token;
                }
            );

        mock.Setup(obj => obj.GeneratePasswordResetTokenAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(
                (UserAccount account) =>
                {
                    var token = Guid.NewGuid().ToString();
                    collection.ResetTokens.Add(account.Email!, token);
                    return token;
                }
            );

        mock.Setup(obj => obj.GetAccountRefreshTokenAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(
                (UserAccount account) =>
                {
                    return collection.RefreshTokens.GetValueOrDefault(account.Email!);
                }
            );

        mock.Setup(obj =>
                obj.ResetPasswordAsync(
                    It.IsAny<UserAccount>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(
                (UserAccount account, string token, string password) =>
                {
                    var dbToken = collection.ResetTokens.GetValueOrDefault(account.Email!);
                    if (dbToken is null || dbToken != token)
                        return IdentityResult.Failed([]);
                    collection.Accounts.ForEach(
                        (item) =>
                        {
                            if (item.Id == account.Id)
                            {
                                item.PasswordHash = password;
                            }
                        }
                    );
                    return IdentityResult.Success;
                }
            );

        mock.Setup(obj =>
                obj.SetAccountRefreshTokenAsync(It.IsAny<UserAccount>(), It.IsAny<string>())
            )
            .ReturnsAsync(
                (UserAccount account, string token) =>
                {
                    collection.RefreshTokens.Add(account.Email!, token);
                    return IdentityResult.Success;
                }
            );

        mock.Setup(obj => obj.UpdateAccountAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(
                (UserAccount account) =>
                {
                    var index = collection.Accounts.FindIndex(item => item.Id == account.Id);
                    if (index < 0)
                        return IdentityResult.Failed([]);
                    collection.Accounts[index] = account;
                    return IdentityResult.Success;
                }
            );

        mock.Setup(obj => obj.VerifyEmailAddressAsync(It.IsAny<UserAccount>(), It.IsAny<string>()))
            .ReturnsAsync(
                (UserAccount account, string token) =>
                {
                    var dbToken = collection.VerifyEmailTokens.GetValueOrDefault(account.Email!);
                    if (dbToken is null || dbToken != token)
                        return IdentityResult.Failed([]);
                    collection.Accounts.ForEach(
                        (item) =>
                        {
                            if (item.Id == account.Id)
                            {
                                item.EmailConfirmed = true;
                            }
                        }
                    );
                    return IdentityResult.Success;
                }
            );

        return mock;
    }
}
