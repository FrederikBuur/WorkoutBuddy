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

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

builder.Configuration
       .SetBasePath(env.ContentRootPath)
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
       .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
       .AddEnvironmentVariables()
       .AddUserSecrets<Program>();

var azureCredentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ManagedIdentityClientId = builder.Configuration.GetValue<string>("Auth:ManagedIdentityId") ?? throw new Exception("Missing configuration: Auth:ManagedIdentityId")
});

var shouldUseKeyVault = builder.Configuration.GetValue<bool>("KeyVault:UseKeyVault", true);
if (shouldUseKeyVault)
{
    var keyVaultUrl = builder.Configuration.GetValue<Uri>("KeyVault:Url") ?? throw new Exception("Missing configuration: KeyVault:Url");

    builder.Configuration.AddAzureKeyVault(keyVaultUrl, azureCredentials);
}

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
    var endpoint = builder.Configuration.GetValue<string>("Cosmos:Uri") ?? throw new Exception("Missing configuration: Cosmos:Uri");
    var primaryKey = builder.Configuration.GetValue<string>("Cosmos:Key") ?? throw new Exception("Missing configuration: Cosmos:Key");
    var dbname = builder.Configuration.GetValue<string>("Cosmos:DbName") ?? throw new Exception("Missing configuration: Cosmos:DbName");

    options.UseCosmos(
        endpoint, 
        primaryKey, 
        dbname);
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
            .AddSwaggerGen(c =>
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
            });

// inject settings
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Auth"));

// firebase authentication
builder.Services.AddSingleton(FirebaseApp.Create());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(
                    JwtBearerDefaults.AuthenticationScheme,
                    (o) => { }
                );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseSwagger()
         .UseSwaggerUI(c =>
         {
             c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Chivado Api");
         });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
