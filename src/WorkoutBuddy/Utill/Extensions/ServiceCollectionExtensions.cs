
using Azure.Identity;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace WorkoutBuddy.Util;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupKeyVaultInjection(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var azureCredentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ExcludeEnvironmentCredential = false,
            ExcludeManagedIdentityCredential = false,
            ExcludeSharedTokenCacheCredential = false,
            ExcludeVisualStudioCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeAzureCliCredential = false,
            ExcludeAzurePowerShellCredential = true,
            ExcludeInteractiveBrowserCredential = true,
        });

        // Inject keyvault secrets
        var shouldUseKeyVault = configuration.GetValue<bool>("KeyVault:UseKeyVault", true);
        if (shouldUseKeyVault)
        {
            var keyVaultUrl = configuration.GetValue<Uri>("KeyVault:Url"); // ?? throw new ArgumentNullException("Missing configuration: KeyVault:Url");

            configuration.AddAzureKeyVault(keyVaultUrl, azureCredentials);
        }
        return services;
    }

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    public static IServiceCollection SetupSwagger(
            this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkoutBuddy", Version = "v1" });
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Provide JWT token",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            options.OperationFilter<BasicAuthOperationsFilter>();
        });
        return services;
    }

    public static IServiceCollection SetupAuthentication(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
    {
        jwtOptions.Authority = configuration["Auth:ValidIssuer"];
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration.GetValue<string>("Auth:ValidIssuer"),
            ValidAudience = configuration.GetValue<string>("Auth:Audience")
        };
        jwtOptions.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Unauthorized request: \n" + context.Exception);
                return Task.CompletedTask;
            }
        };
    });
        return services;
    }

    public static IServiceCollection SetupEntityFramework(
    this IServiceCollection services,
    ConfigurationManager configuration)
    {
        services.AddDbContext<DataContext>(options =>
        {
            var sqlDbConnectionString = configuration.GetConnectionString("SQL") ?? throw new ArgumentNullException("Missing SQL connectionstring");
            options.UseSqlServer(sqlDbConnectionString, sqlServerOptions =>
            {
                sqlServerOptions.CommandTimeout(30);
            });
        });
        return services;
    }

    public static IServiceCollection SetupFirebase(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var firebaseConfig = configuration.GetValue<string>("Auth:FirebaseConfig"); // ?? throw new ArgumentNullException("missing firebase config");
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson(firebaseConfig)
        });
        return services;
    }
}