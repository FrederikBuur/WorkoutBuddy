global using WorkoutBuddy.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using WorkoutBuddy.Services;
using WorkoutBuddy.Features;
using Microsoft.OpenApi.Models;

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
// https://www.youtube.com/watch?v=z46lqVOv1hQ&ab_channel=ThumbIKR-ProgrammingExamples
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkoutBuddy", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Provide JWT token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
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

// setup firebase authentication
// Convert firebase config json to string: https://tools.knowledgewalls.com/json-to-string
var firebaseConfig = builder.Configuration.GetValue<string>("Auth:FirebaseConfig"); // ?? throw new ArgumentNullException("missing firebase config");
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(firebaseConfig)
});

builder.Services.AddHttpClient<GoogleJwtProvider, GoogleJwtProvider>(httpClient =>
{
    var baseAddress = builder.Configuration["Auth:TokenUri"] ?? throw new ArgumentNullException("Missing: Auth:TokenUri");
    var apiKey = builder.Configuration["Auth:FirebaseApiKey"] ?? throw new ArgumentException("Missing firebase api key");
    httpClient.BaseAddress = new Uri($"{baseAddress}?key={apiKey}");
});

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
    {
        jwtOptions.Authority = builder.Configuration["Auth:ValidIssuer"];
        jwtOptions.Audience = builder.Configuration["Auth:Audience"];
        jwtOptions.TokenValidationParameters.ValidIssuer = builder.Configuration["Auth:ValidIssuer"];
    });

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(
//         JwtBearerDefaults.AuthenticationScheme,
//         (options) => { }
//     );

// setup services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddScoped<ProfileService, ProfileService>();
builder.Services.AddScoped<WorkoutDetailService, WorkoutDetailService>();
builder.Services.AddScoped<ExerciseDetailService, ExerciseDetailService>();
builder.Services.AddScoped<WorkoutService, WorkoutService>();
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
