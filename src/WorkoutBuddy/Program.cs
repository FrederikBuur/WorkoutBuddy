global using WorkoutBuddy.Data;
using WorkoutBuddy.Services;
using WorkoutBuddy.Features;
using WorkoutBuddy.Util;
using WorkoutBuddy.Util.ErrorHandling;
using WorkoutBuddy.Features.Authentication;

var builder = WebApplication.CreateBuilder(args);

var _logger = ApplicationLogging.CreateLogger("Program");

// Setup infrastructure
builder.Services
    .SetupKeyVaultInjection(builder.Configuration)
    .SetupSwagger()
    .SetupAuthentication(builder.Configuration)
    .SetupEntityFramework(builder.Configuration)
    .SetupFirebase(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOutputCache(); // can be faulty if multiple instances
builder.Services.AddHttpContextAccessor();
if (!builder.Environment.IsDevelopment())
{
    builder.Services.SetupApplicationInsights(builder.Configuration);
}

// Add services to the container.
builder.Services.AddScoped<UserService, UserService>()
    .AddScoped<ProfilesService, ProfilesService>()
    .AddScoped<WorkoutDetailsService, WorkoutDetailsService>()
    .AddScoped<ExerciseDetailsService, ExerciseDetailsService>()
    .AddScoped<WorkoutsService, WorkoutsService>()
    .AddScoped<SessionsService, SessionsService>()
    .AddScoped<AuthService, AuthService>();

// Setup http clients
builder.Services.AddHttpClient<GoogleJwtProvider, GoogleJwtProvider>(httpClient =>
{
    var identityUri = builder.Configuration.GetValue<string>("Auth:IdentityUri");
    var firebaseApiKey = builder.Configuration.GetValue<string>("Auth:FirebaseApiKey");

    if (string.IsNullOrEmpty(identityUri)) _logger.LogError($"{nameof(identityUri)} is null/empty");
    if (string.IsNullOrEmpty(firebaseApiKey)) _logger.LogError($"{nameof(firebaseApiKey)} is null/empty");

    var baseAddress = identityUri;
    var apiKey = firebaseApiKey;
    httpClient.BaseAddress = new Uri($"{baseAddress}?key={apiKey}");
});
builder.Services.AddHttpClient<GoogleRefreshProvider, GoogleRefreshProvider>(httpClient =>
{
    var tokenUri = builder.Configuration.GetValue<string>("Auth:TokenUri");
    var firebaseApiKey = builder.Configuration.GetValue<string>("Auth:FirebaseApiKey");

    if (string.IsNullOrEmpty(tokenUri)) _logger.LogError($"{nameof(tokenUri)} is null/empty");
    if (string.IsNullOrEmpty(firebaseApiKey)) _logger.LogError($"{nameof(firebaseApiKey)} is null/empty");

    var baseAddress = tokenUri;
    var apiKey = firebaseApiKey;
    httpClient.BaseAddress = new Uri($"{baseAddress}?key={apiKey}");
});

// CORS
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost3000",
            builder => builder
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader() // Allow any header, including Content-Type
                .AllowCredentials()); // If you need to support credentials
    });
}

var app = builder.Build();

switch (args.FirstOrDefault())
{
    case "seedDb":
        await app.SeedDb();
        return;
    case "migrate-latest":
        await app.MigrateLatest();
        return;
    case "migrate-down":
        await app.MigrateDown();
        return;
    default:
        RunApp(app);
        return;
}

static void RunApp(WebApplication app)
{
    if (app.Environment.IsProduction())
    {
        // if unexcpected exception occurs, reroute to '/error' see ErrorController.cs
        app.UseExceptionHandler("/error");
    }
    else if (app.Environment.IsDevelopment())
    {
        app.UseCors("AllowLocalhost3000");
    }

    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/swagger/{documentname}/swagger.json";
    })
    .UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/api/swagger/v1/swagger.json", "WorkoutBuddy Api");
        c.RoutePrefix = "api/swagger";
    });

    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
    //app.UseOutputCache(); // can be faulty if multiple instances of app running

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.MapControllers();

    app.Run();
}