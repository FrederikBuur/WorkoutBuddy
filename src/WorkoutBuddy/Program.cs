global using WorkoutBuddy.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using workouts;
using FirebaseAdmin;
using WorkoutBuddy.Authentication;
using Google.Apis.Auth.OAuth2;
using WorkoutBuddy.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using WorkoutBuddy.Features;

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

// Inject keyvault secrets
var shouldUseKeyVault = builder.Configuration.GetValue<bool>("KeyVault:UseKeyVault", true);
if (shouldUseKeyVault)
{
    var keyVaultUrl = builder.Configuration.GetValue<Uri>("KeyVault:Url"); // ?? throw new ArgumentNullException("Missing configuration: KeyVault:Url");

    builder.Configuration.AddAzureKeyVault(keyVaultUrl, azureCredentials);
}

// Setup EF Core to SQL db connection
builder.Services.AddDbContext<DataContext>(options =>
{
    var sqlDbConnectionString = builder.Configuration.GetConnectionString("SQL") ?? throw new ArgumentNullException("Missing SQL connectionstring");
    options.UseSqlServer(sqlDbConnectionString, sqlServerOptions =>
    {
        sqlServerOptions.CommandTimeout(30);
    });
});

builder.Services.AddEndpointsApiExplorer();

// setup swagger options
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerAuthOptions>();

// setup application insight logging
if (env.EnvironmentName != "Local")
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.DeveloperMode = false;
        options.ConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING"); // ?? throw new ArgumentNullException("Missing: APPLICATIONINSIGHTS_CONNECTION_STRING");
    });
}

// setup firebase authentication
// Convert firebase config json to string: https://tools.knowledgewalls.com/json-to-string
var firebaseConfig = builder.Configuration.GetValue<string>("Auth:FirebaseConfig"); // ?? throw new ArgumentNullException("missing firebase config");
builder.Services.Configure<FirebaseAuthOptions>(builder.Configuration.GetSection("Auth"));
builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(firebaseConfig)
}));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(
        JwtBearerDefaults.AuthenticationScheme,
        (options) => { }
    );

// setup services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<WorkoutDetailService, WorkoutDetailService>();
builder.Services.AddOutputCache(); // can be faulty if multiple instances

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
        });

    app.UseHttpsRedirection();
    app.UseOutputCache(); // can be faulty if multiple instances of app running

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
