using FinBooKeAPI.Collections.AccountCollection;
using FinBooKeApp.Api.Configuration.Authentication;
using FinBooKeApp.Api.Configuration.Database;
using FinBookeApp.Api.Configuration.Documentation;
using FinBooKeApp.Api.Configuration.Localization;
using FinBooKeApp.Api.Configuration.Redaction;
using FinBooKeApp.Api.Configuration.Settings;
using FinBooKeApp.Api.Configuration.Version;
using FinBooKeApp.Api.Middleware;
using FinBooKeApp.Core.Services.Authentication;
using FinBooKeApp.Core.Services.DataImport;
using FinBooKeApp.Core.Services.Profile;
using FinBooKeApp.Core.Shared.Authentication.Interfaces;
using FinBooKeApp.Core.Shared.Authentication.Providers;
using FinBooKeApp.Core.Shared.Email.Interfaces;
using FinBooKeApp.Core.Shared.Email.Providers;
using FinBooKeApp.Core.Shared.FileSystem.Interfaces;
using FinBooKeApp.Core.Shared.FileSystem.Providers;
using FinBooKeApp.Core.Shared.Parsing.Interfaces;
using FinBooKeApp.Core.Shared.Parsing.Providers;
using FinBooKeApp.Core.Shared.Security.Interfaces;
using FinBooKeApp.Core.Shared.Security.Providers;
using FinBooKeApp.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Compliance.Redaction;

var builder = WebApplication.CreateBuilder(args);

// Add app configurations.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
});
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
builder.Services.AddSingleton<IRedactorProvider, StarRedactorProvider>();

// Collections
builder.Services.AddScoped<IAccountCollection, AccountCollection>();

// Logic
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IClaimProvider, ClaimProvider>();
builder.Services.AddScoped<IEmailProvider, EmailProvider>();
builder.Services.AddScoped<IEmailTemplateBuilder, EmailTemplateBuilder>();
builder.Services.AddScoped<IDataProtection, DataProtection>();
builder.Services.AddScoped<IHashProvider, HashProvider>();
builder.Services.AddScoped<IFileSystem, FileSystem>();
builder.Services.AddScoped<IUploadSystem, UploadFileSystem>();
builder.Services.AddScoped<JsonParser>();
builder.Services.AddScoped<XmlParser>();
builder.Services.AddScoped<CsvParser>();
builder.Services.AddScoped<IParserFactory, ParserFactory>();

// Services that provides key functionality
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IDataImportService, DataImportService>();
builder.Services.AddTransient<ExceptionHandling>();

// Import test data into database
await builder.Services.ImportData();

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
