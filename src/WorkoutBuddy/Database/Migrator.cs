using Microsoft.EntityFrameworkCore;

namespace WorkoutBuddy.Data;

internal static class Migrator
{
    internal static async Task Migrate(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        context.Database.SetCommandTimeout(300);
        await context.Database.MigrateAsync();
    }
}
