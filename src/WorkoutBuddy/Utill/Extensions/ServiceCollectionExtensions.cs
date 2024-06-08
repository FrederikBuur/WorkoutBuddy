
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
    private static ILogger _logger;
    static ServiceCollectionExtensions()
    {
        _logger = ApplicationLogging.CreateLogger("ServiceCollectionExtensions");
    }

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
        var shouldUseKeyVault = configuration.GetValue("KeyVault:UseKeyVault", true);
        if (shouldUseKeyVault)
        {
            var keyVaultUrl = configuration.GetValue<Uri>("KeyVault:Url");

            if (keyVaultUrl is not null)
            {
                configuration.AddAzureKeyVault(keyVaultUrl, azureCredentials);
            }
            else
            {
                _logger.LogError($"{nameof(SetupKeyVaultInjection)}: KeyVault:Url from configuration was null");
                // Console.WriteLine($"{nameof(SetupKeyVaultInjection)}: KeyVault:Url from configuration was null");
            }
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
                _logger.LogWarning("Unauthorized request: \n" + context.Exception);
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
        var firebaseConfig = configuration.GetValue<string>("Auth:FirebaseConfig");

        if (!string.IsNullOrEmpty(firebaseConfig))
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(firebaseConfig)
            });
        }
        else
        {
            _logger.LogError($"{nameof(SetupFirebase)}: Auth:FirebaseConfig form config was null");
            // Console.WriteLine($"{nameof(SetupFirebase)}: Auth:FirebaseConfig form config was null");
        }
        return services;
    }
    public static IServiceCollection SetupApplicationInsights(this IServiceCollection services, IConfiguration configuration) =>
        services.AddApplicationInsightsTelemetry(options =>
            {
                options.DeveloperMode = false;
                options.InstrumentationKey = configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");
            });

}