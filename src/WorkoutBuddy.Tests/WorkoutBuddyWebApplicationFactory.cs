using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Configuration;
using WorkoutBuddy.Data;

namespace WorkoutBuddy.Tests;

internal class WorkoutBuddyWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Remove(services.Single(s => s.ServiceType == typeof(DbContextOptions<DataContext>)));

            //var connectionString = SqlTestConnectionString();
            //services.AddSqlServer<DataContext>(connectionString, builder =>
            //{
            //builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null);
            //});


            services.AddAuthentication(TestAuthHandler.TestAuthScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.TestAuthScheme,
                options => { });

            services.AddDbContext<DataContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // clear dbContext before each integration test
            var serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.EnsureCreated();
        });

    }

    private static string SqlTestConnectionString()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.{environment}.json")
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("SQL") ?? throw new ConfigurationErrorsException("Missing SQLTest connection string");

        return connectionString;
    }
}
