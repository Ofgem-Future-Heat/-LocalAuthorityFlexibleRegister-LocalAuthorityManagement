using Microsoft.EntityFrameworkCore;
using Ofgem.LAF.SharedLibrary.Models;

namespace Ofgem.API.LAF.LocalAuthorityManagement.Data.Extensions;

public static class Paging
{

    public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query,
        int page, int pageSize) where T : class
    {

        var result = new PagedResult<T>();
        result.CurrentPage = page;
        result.PageSize = pageSize;
        result.RowCount = await query.CountAsync();

        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = await query.Skip(skip).Take(pageSize).ToListAsync();
        return result;
    }
}