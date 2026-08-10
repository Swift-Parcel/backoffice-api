using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Infrastructure.Persistence.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedListAsync<T>(
        this IQueryable<T> source, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        
        if (count == 0)
        {
            return new PagedResult<T>(Array.Empty<T>(), 0, pageNumber, pageSize);
        }

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, count, pageNumber, pageSize);
    }
}