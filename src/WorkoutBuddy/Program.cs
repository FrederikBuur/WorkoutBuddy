global using WorkoutBuddy.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using workouts;
using FirebaseAdmin;
using Microsoft.OpenApi.Models;
using WorkoutBuddy.Authentication;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using WorkoutBuddy.Controllers;
using Google.Apis.Auth.OAuth2;
using WorkoutBuddy.Services;
using WorkoutBuddy.Features.WorkoutModel;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

builder.Configuration
       .SetBasePath(env.ContentRootPath)
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
       .AddEnvironmentVariables()
       .AddUserSecrets<Program>();

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

// Setup keyvault secrets
var shouldUseKeyVault = builder.Configuration.GetValue<bool>("KeyVault:UseKeyVault", true);
if (shouldUseKeyVault)
{
    var keyVaultUrl = builder.Configuration.GetValue<Uri>("KeyVault:Url"); // ?? throw new ArgumentNullException("Missing configuration: KeyVault:Url");

    builder.Configuration.AddAzureKeyVault(keyVaultUrl, azureCredentials);
}

// Setup EF Core
var endpoint = builder.Configuration.GetValue<string>("Cosmos:Uri"); // ?? throw new ArgumentNullException("Missing configuration: Cosmos:Uri");
var primaryKey = builder.Configuration.GetValue<string>("Cosmos:Key"); // ?? throw new ArgumentNullException("Missing configuration: Cosmos:Key");
var dbname = builder.Configuration.GetValue<string>("Cosmos:DbName"); // ?? throw new ArgumentNullException("Missing configuration: Cosmos:DbName");
builder.Services.AddDbContext<DataContext>(options =>
{
    // todo still cant use managed identity to connect to cosmos with EF
    options.UseCosmos(
        endpoint,
        primaryKey,
        dbname
    );
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Time To Work", Version = "v1" });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
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
    //c.SchemaFilter<EnumSchemaFilter>(); // todo show enums in swagger
});

// setup application insight logging
if (env.EnvironmentName != "Local")
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.DeveloperMode = false;
        options.ConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING"); // ?? throw new ArgumentNullException("Missing: APPLICATIONINSIGHTS_CONNECTION_STRING");
    });
}

// firebase authentication
// JSON TO STRING CONVERTER: https://tools.knowledgewalls.com/json-to-string
// step 1: https://www.youtube.com/watch?v=edqmYmcLnjE&t=618s&ab_channel=SingletonSean - DONE
// step 2: https://www.youtube.com/watch?v=jkTaHb0M4nw&ab_channel=SingletonSean - TODO
var firebaseConfig = builder.Configuration.GetValue<string>("Auth:FirebaseConfig"); // ?? throw new ArgumentNullException("missing firebase config");
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Auth"))
    .AddSingleton(FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromJson(firebaseConfig)
    }))
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(
        JwtBearerDefaults.AuthenticationScheme,
        (options) => { }
    );

// setup services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<WorkoutService, WorkoutService>();

var app = builder.Build();

switch (args.FirstOrDefault())
{
    case "initAndSeedDb":
        await app.InitAndSeedDb();
        return;
    case null:
        RunApp(app);
        return;
    case var arg: throw new ArgumentException($"Unknown command-line argument: {arg}");
}

static void RunApp(WebApplication app)
{
    app.UseExceptionHandler("/error")
        .UseSwagger()
        .UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Chivado Api");
        })
        .UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
