
using Microsoft.EntityFrameworkCore;

namespace WorkoutBuddy.Util;

public static class IQueryableExtensions
{
    public static async Task<Paginated<T>> GetPage<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = query.Count();
        var totalPages = count / pageSize;
        if (count % pageSize != 0) totalPages++;

        var pagedWorkoutLogs = await query.Skip(Math.Max(0, pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Paginated<T>(
            totalPages: totalPages,
            currentPage: pageNumber,
            pageSize: pageSize,
            totalItems: count,
            lastPage: totalPages == pageNumber,
            items: pagedWorkoutLogs
        );
    }
}