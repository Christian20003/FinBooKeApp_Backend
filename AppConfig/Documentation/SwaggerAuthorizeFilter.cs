using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FinBookeAPI.AppConfig.Documentation;

public class SwaggerAuthorizeFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.DeclaringType is null)
            return;

        var classAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true);
        var methodAttributes = context.MethodInfo.GetCustomAttributes(true);

        var hasAuthorize =
            classAttributes.OfType<AuthorizeAttribute>().Any()
            || methodAttributes.OfType<AuthorizeAttribute>().Any();
        var hasAllowAnonymous =
            classAttributes.OfType<AllowAnonymousAttribute>().Any()
            || methodAttributes.OfType<AllowAnonymousAttribute>().Any();

        if (!hasAuthorize && hasAllowAnonymous)
        {
            operation.Security =
            [
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer")] = [],
                },
            ];
        }
    }
}
