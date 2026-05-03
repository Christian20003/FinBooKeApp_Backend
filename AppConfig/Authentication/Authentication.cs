using System.Text;
using FinBookeAPI.AppConfig.Database;
using FinBookeAPI.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FinBookeAPI.AppConfig.Authentication;

public static class Authentication
{
    public static IServiceCollection AddAuthenticationConfig(
        this IServiceCollection services,
        IConfiguration _configuration
    )
    {
        // Add an identity system
        services
            .AddIdentityCore<UserAccount>()
            .AddSignInManager()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        // Add authentication restrictions
        services.Configure<IdentityOptions>(options =>
        {
            // Password restrictions
            options.Password.RequiredLength = 10;
            options.Password.RequiredUniqueChars = 5;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;

            // User restrictions
            options.User.RequireUniqueEmail = true;

            // Request restrictions
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        });

        // Add authentication provider
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        authBuilder.AddJwtBearer(options =>
        {
            var jwt = _configuration.GetSection("JwtConfig");
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var secret = jwt["AccessTokenSecret"];
            if (issuer == null || audience == null || secret == null)
            {
                throw new ApplicationException("Missing authentication data in configuration");
            }
            options.SaveToken = true;
            //TODO: Should be changed after development
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidIssuer = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            };
        });
        return services;
    }
}
