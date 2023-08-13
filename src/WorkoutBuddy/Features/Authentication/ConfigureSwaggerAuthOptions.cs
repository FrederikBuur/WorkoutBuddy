using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WorkoutBuddy.Authentication;

public class ConfigureSwaggerAuthOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions c)
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Time To Work", Version = "v1" });
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Provide valid login for WorkoutBuddy",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Password = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri("/v1/auth", UriKind.Relative),
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        { "returnSecureToken", new OpenApiBoolean(true) },
                    },
                }
            }
        });
        c.OperationFilter<AuthorizeCheckOperationFilter>();
    }
}