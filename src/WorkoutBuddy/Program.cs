global using WorkoutBuddy.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using WorkoutBuddy.Services;
using WorkoutBuddy.Features;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
    {
        jwtOptions.Authority = builder.Configuration["Auth:ValidIssuer"];
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Auth:ValidIssuer"),
            ValidAudience = builder.Configuration.GetValue<string>("Auth:Audience")
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

// Setup EF Core to SQL db connection
builder.Services.AddDbContext<DataContext>(options =>
{
    var sqlDbConnectionString = builder.Configuration.GetConnectionString("SQL") ?? throw new ArgumentNullException("Missing SQL connectionstring");
    options.UseSqlServer(sqlDbConnectionString, sqlServerOptions =>
    {
        sqlServerOptions.CommandTimeout(30);
    });
});

// Setup Firebase
var firebaseConfig = builder.Configuration.GetValue<string>("Auth:FirebaseConfig"); // ?? throw new ArgumentNullException("missing firebase config");
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(firebaseConfig)
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOutputCache(); // can be faulty if multiple instances
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<GoogleJwtProvider, GoogleJwtProvider>(httpClient =>
{
    var baseAddress = builder.Configuration["Auth:TokenUri"] ?? throw new ArgumentNullException("Missing: Auth:TokenUri");
    var apiKey = builder.Configuration["Auth:FirebaseApiKey"] ?? throw new ArgumentException("Missing firebase api key");
    httpClient.BaseAddress = new Uri($"{baseAddress}?key={apiKey}");
});
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddScoped<ProfileService, ProfileService>();
builder.Services.AddScoped<WorkoutDetailService, WorkoutDetailService>();
builder.Services.AddScoped<ExerciseDetailService, ExerciseDetailService>();
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
    if (app.Environment.IsProduction())
    {
        // if unexcpected exception occurs, reroute to '/error' see ErrorController.cs
        app.UseExceptionHandler("/error");
    }

    app.UseSwagger()
    .UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/v1/swagger.json", "WorkoutBuddy Api");
    });

    app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    app.UseHttpsRedirection();
    app.UseOutputCache(); // can be faulty if multiple instances of app running

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}