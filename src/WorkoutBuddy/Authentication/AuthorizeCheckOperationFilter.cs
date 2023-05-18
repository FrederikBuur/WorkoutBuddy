using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WorkoutBuddy.Authentication;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requiredScopes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                 .OfType<AuthorizeAttribute>()
                 .Select(attr => attr.Policy)
                 .Distinct();

        if (requiredScopes?.Any() == true)
        {
            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
             {
                 new OpenApiSecurityRequirement
                 {
                     [ oAuthScheme ] = requiredScopes.ToList()
                 }
             };
        }
    }
}