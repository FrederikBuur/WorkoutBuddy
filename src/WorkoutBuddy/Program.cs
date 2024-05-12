global using WorkoutBuddy.Data;
using WorkoutBuddy.Services;
using WorkoutBuddy.Features;
using WorkoutBuddy.Util;
using WorkoutBuddy.Util.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddScoped<UserService, UserService>()
    .AddScoped<ProfileService, ProfileService>()
    .AddScoped<WorkoutDetailService, WorkoutDetailService>()
    .AddScoped<ExerciseDetailService, ExerciseDetailService>()
    .AddScoped<WorkoutService, WorkoutService>()
    .AddScoped<SessionService, SessionService>();

// Setup http clients
builder.Services.AddHttpClient<GoogleJwtProvider, GoogleJwtProvider>(httpClient =>
{
    var baseAddress = builder.Configuration["Auth:TokenUri"] ?? throw new ArgumentNullException("Missing: Auth:TokenUri");
    var apiKey = builder.Configuration["Auth:FirebaseApiKey"] ?? throw new ArgumentException("Missing firebase api key");
    httpClient.BaseAddress = new Uri($"{baseAddress}?key={apiKey}");
});

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

    app.UseHttpsRedirection();
    app.UseOutputCache(); // can be faulty if multiple instances of app running

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.MapControllers();

    app.Run();
}