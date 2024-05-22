using Microsoft.EntityFrameworkCore;

namespace WorkoutBuddy.Data;

internal static class Migrator
{
    internal static async Task Migrate(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Database.SetCommandTimeout(300);

        if (await context.Database.EnsureCreatedAsync())
        {
            Console.WriteLine("Db was created");
        }
        else
        {
            Console.WriteLine("Db already exists");
        }

        await context.Database.MigrateAsync();
    }
}
