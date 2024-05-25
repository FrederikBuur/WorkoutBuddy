using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutBuddy.Data;

internal static class Migrator
{
    internal static async Task MigrateLatest(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Database.SetCommandTimeout(300);

        // 'EnsureCreatedAsync' used during initial development, cvant use migrations. Swap to MigrateAsync When more stable state reached
        if (await context.Database.EnsureCreatedAsync())
        {
            Console.WriteLine("Db was created");
        }
        else
        {
            Console.WriteLine("Db already exists");
        }

        //await context.Database.MigrateAsync();
    }

    internal static async Task MigrateDown(this WebApplication app)
    {

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var migrator = context.GetInfrastructure().GetRequiredService<IMigrator>();
        context.Database.SetCommandTimeout(300);

        var migrations = await context.Database.GetAppliedMigrationsAsync();
        var previousMigration = migrations.ElementAt(migrations.Count() - 1);

        await migrator.MigrateAsync(previousMigration);
    }
}
