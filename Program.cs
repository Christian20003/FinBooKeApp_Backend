using FinBookeAPI.AppConfig.Authentication;
using FinBookeAPI.AppConfig.Database;
using FinBookeAPI.AppConfig.Documentation;
using FinBookeAPI.AppConfig.Localization;
using FinBookeAPI.AppConfig.Redaction;
using FinBookeAPI.AppConfig.Settings;
using FinBookeAPI.AppConfig.Version;
using FinBooKeAPI.Collections.AccountCollection;
using FinBookeAPI.Collections.AmountCollection;
using FinBookeAPI.Collections.CategoryCollection;
using FinBookeAPI.Collections.PaymentMethodCollection;
using FinBookeAPI.Collections.TokenCollection;
using FinBooKeAPI.Logic.Authentication;
using FinBooKeAPI.Logic.Email;
using FinBookeAPI.Middleware;
using FinBookeAPI.Models.Wrapper;
using FinBookeAPI.Services.AmountManagement;
using FinBookeAPI.Services.Authentication;
using FinBookeAPI.Services.CategoryType;
using FinBookeAPI.Services.Email;
using FinBookeAPI.Services.Payment;
using FinBookeAPI.Services.SecurityUtility;
using FinBookeAPI.Services.Token;
using FinBookeAPI.Services.Upload;
using Microsoft.Extensions.Compliance.Redaction;

var builder = WebApplication.CreateBuilder(args);

// Add app configurations.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddVersioningConfig();
builder.Services.AddSettingsConfig(builder.Configuration);
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddDbContext<DataDbContext>();
builder.Services.AddAuthenticationConfig(builder.Configuration);
builder.Services.AddRedactionConfig();
builder.Services.AddLoggingConfig(builder.Configuration);
builder.Services.AddLocalizationConfig();
builder.Services.AddSwaggerConfig();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Wrapper
builder.Services.AddSingleton<IDataProtection, DataProtection>();
builder.Services.AddSingleton<IRedactorProvider, StarRedactorProvider>();
builder.Services.AddScoped<IAccountManager, AccountManager>();

// Collections
builder.Services.AddScoped<ITokenCollection, TokenCollection>();
builder.Services.AddScoped<ICategoryCollection, CategoryCollection>();
builder.Services.AddScoped<IPaymentMethodCollection, PaymentMethodCollection>();
builder.Services.AddScoped<IAmountCollection, AmountCollection>();
builder.Services.AddScoped<IAccountCollection, AccountCollection>();

// Logic
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IClaimProvider, ClaimProvider>();
builder.Services.AddScoped<IEmailProvider, EmailProvider>();

// Services that provides additional functionality
builder.Services.AddScoped<ISecurityUtilityService, SecurityUtilityService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Services that provides key functionality
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IAmountManagementService, AmountManagementService>();
builder.Services.AddTransient<ExceptionHandling>();

// Import test data into database - deprecated
//await builder.Services.ImportUsers();
//await builder.Services.ImportData();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}
app.UseMiddleware<ExceptionHandling>();
app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
