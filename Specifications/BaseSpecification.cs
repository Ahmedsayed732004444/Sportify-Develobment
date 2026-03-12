using System.Linq.Expressions;

namespace Sportiva.Specifications;

public abstract class BaseSpecification<T>
{
    public Expression<Func<T, bool>>?        Criteria     { get; protected set; }
    public List<Expression<Func<T, object>>> Includes     { get; } = [];
    public List<string>                      IncludeStrings { get; } = [];
    public Expression<Func<T, object>>?      OrderBy      { get; protected set; }
    public Expression<Func<T, object>>?      OrderByDesc  { get; protected set; }
    public int?                              Take         { get; protected set; }
    public int?                              Skip         { get; protected set; }

    protected void AddInclude(Expression<Func<T, object>> include)
        => Includes.Add(include);

    // للـ ThenInclude زي "Posts.Comments"
    protected void AddInclude(string includeString)
        => IncludeStrings.Add(includeString);

    protected void ApplyOrderBy(Expression<Func<T, object>> orderBy)
        => OrderBy = orderBy;

    protected void ApplyOrderByDesc(Expression<Func<T, object>> orderByDesc)
        => OrderByDesc = orderByDesc;

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }
}
