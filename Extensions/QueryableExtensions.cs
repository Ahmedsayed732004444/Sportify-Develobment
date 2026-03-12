using Sportiva.Contracts.Common;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Sportiva.Extensions;
public static class QueryableExtensions
{
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query,RequestFilters filters,Expression<Func<T, bool>>? searchPredicate = null)
    {
        // Search
        if (!string.IsNullOrEmpty(filters.SearchValue) && searchPredicate is not null)
            query = query.Where(searchPredicate);

        // Sort
        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        return query;
    }

    public static Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query,RequestFilters filters, CancellationToken cancellationToken = default)
        => PaginatedList<T>.CreateAsync(query, filters.PageNumber, filters.PageSize, cancellationToken);
}



//1. أي Service عندك فيها pagination
//csharp
//// بدل الكود الطويل ده
//var query = _context.JobSubmissions
//    .Where(js => js.JobId == jobId);

//if (!string.IsNullOrEmpty(filters.SearchValue))
//    query = query.Where(x => x.Notes != null && x.Notes.Contains(filters.SearchValue));

//if (!string.IsNullOrEmpty(filters.SortColumn))
//    query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

//var source = query.ProjectToType<ApplyJobResponse>().AsNoTracking();
//var submissions = await PaginatedList<ApplyJobResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);
//csharp

//// بقى كده
//var submissions = await _context.JobSubmissions
//    .Where(js => js.JobId == jobId)
//    .ApplyFilters(filters,
//        searchPredicate: x => x.Notes != null && x.Notes.Contains(filters.SearchValue!))
//    .ProjectToType<ApplyJobResponse>()
//    .AsNoTracking()
//    .ToPaginatedListAsync(filters, cancellationToken);



//return Result.Success(submissions);
//2. Service تانية(مثلاً Jobs)
//csharp
//var jobs = await _context.Jobs
//    .Where(j => j.CompanyId == companyId)
//    .ApplyFilters(filters,
//        searchPredicate: x => x.Title.Contains(filters.SearchValue!))
//    .ProjectToType<JobResponse>()
//    .AsNoTracking()
//    .ToPaginatedListAsync(filters, cancellationToken);


//return Result.Success(jobs);
//3. من غير Search(لو مش محتاج تبحث)
//csharp
//var jobs = await _context.Jobs
//    .Where(j => j.CompanyId == companyId)
//    .ApplyFilters(filters) // من غير searchPredicate
//    .ProjectToType<JobResponse>()
//    .AsNoTracking()
//    .ToPaginatedListAsync(filters, cancellationToken);
//4. لو عندك Search في أكتر من Column
//csharp
//.ApplyFilters(filters,
//    searchPredicate: x => 
//        x.Title.Contains(filters.SearchValue!) || 
//        x.Description.Contains(filters.SearchValue!))