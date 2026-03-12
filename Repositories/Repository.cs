using System.Linq.Expressions;
using Sportiva.Specifications;

namespace Sportiva.Repositories;
public class Repository<T>(ApplicationDbContext context) : IRepository<T>where T : class
{
    private readonly DbSet<T> _table = context.Set<T>();

    // ── Reads ──────────────────────────────────────────────────────────────

    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _table.FindAsync([id], ct).ConfigureAwait(false);

    public async Task<T?> GetAsync(Expression<Func<T, bool>> criteria,
                                   CancellationToken ct = default)
        => await _table.AsNoTracking()
                       .FirstOrDefaultAsync(criteria, ct)
                       .ConfigureAwait(false);

    // ── Specification Reads ────────────────────────────────────────────────

    public async Task<T?> GetAsync(BaseSpecification<T> spec, CancellationToken ct = default)
        => await SpecificationEvaluator<T>
            .GetQuery(_table.AsNoTracking(), spec)
            .FirstOrDefaultAsync(ct)
            .ConfigureAwait(false);

    public async Task<List<T>> GetAllAsync(BaseSpecification<T> spec, CancellationToken ct = default)
        => await SpecificationEvaluator<T>
            .GetQuery(_table.AsNoTracking(), spec)
            .ToListAsync(ct)
            .ConfigureAwait(false);

    public async Task<int> CountAsync(BaseSpecification<T> spec, CancellationToken ct = default)
        => await SpecificationEvaluator<T>
            .GetQuery(_table.AsNoTracking(), spec)
            .CountAsync(ct)
            .ConfigureAwait(false);

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? criteria = null,
                                           CancellationToken ct = default)
    {
        var query = _table.AsNoTracking();
        if (criteria is not null)
            query = query.Where(criteria);

        return await query.ToListAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// بيرجع IQueryable تقدر تستخدمه مع ApplyFilters و ToPaginatedListAsync
    /// بدون ما تجيب الداتا من الـ DB على طول
    /// </summary>
    public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? criteria = null)
    {
        var query = _table.AsNoTracking().AsQueryable();
        return criteria is null ? query : query.Where(criteria);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> criteria,
                                     CancellationToken ct = default)
        => await _table.AnyAsync(criteria, ct).ConfigureAwait(false);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null,
                                      CancellationToken ct = default)
        => criteria is null
            ? await _table.CountAsync(ct).ConfigureAwait(false)
            : await _table.CountAsync(criteria, ct).ConfigureAwait(false);

    // ── Writes (مع try/catch لأنها بتغير state) ────────────────────────────

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            await _table.AddAsync(entity, ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to add entity of type {typeof(T).Name}.", ex);
        }
    }

    public async Task AddRangeAsync(IEnumerable<T> entities,
                                    CancellationToken ct = default)
    {
        try
        {
            await _table.AddRangeAsync(entities, ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to add range of {typeof(T).Name}.", ex);
        }
    }

    public void Update(T entity)
    {
        try
        {
            _table.Update(entity);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to update entity of type {typeof(T).Name}.", ex);
        }
    }

    public void Remove(T entity)
    {
        try
        {
            _table.Remove(entity);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to remove entity of type {typeof(T).Name}.", ex);
        }
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        try
        {
            _table.RemoveRange(entities);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to remove range of {typeof(T).Name}.", ex);
        }
    }
}