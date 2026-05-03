using System.Reflection;
using Microsoft.OpenApi;

namespace FinBookeAPI.AppConfig.Documentation;

public static class Swagger
{
    private static readonly string VERSION = "v1";

    private static readonly string NAME = "FinBooke Web-API";

    private static readonly string ENDPOINT = "/swagger/v1/swagger.json";

    private static readonly OpenApiInfo API_INFO = new() { Title = NAME, Version = VERSION };

    private static readonly OpenApiSecurityScheme SECURITY_SCHEME = new()
    {
        Description = "Add a JWT token (Bearer scheme) to the header. Example: 'Bearer [TOKEN]'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    };

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(VERSION, API_INFO);

            options.AddSecurityDefinition(SECURITY_SCHEME.Scheme, SECURITY_SCHEME);

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(SECURITY_SCHEME.Scheme!, doc)] = [],
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
            options.OperationFilter<SwaggerAuthorizeFilter>();
        });

        return services;
    }

    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(ENDPOINT, NAME);
        });
        return app;
    }
}
