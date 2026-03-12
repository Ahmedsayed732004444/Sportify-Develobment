namespace Sportiva.Specifications;

public static class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, BaseSpecification<T> spec)
    {
        var query = inputQuery;

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        query = spec.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        query = spec.IncludeStrings
            .Aggregate(query, (current, include) => current.Include(include));

        if (spec.OrderBy is not null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDesc is not null)
            query = query.OrderByDescending(spec.OrderByDesc);

        if (spec.Skip is not null)
            query = query.Skip(spec.Skip.Value);

        if (spec.Take is not null)
            query = query.Take(spec.Take.Value);

        return query;
    }
}
