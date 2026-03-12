using System.Linq.Expressions;

namespace Sportiva.Repositories;
public interface IRepository<T> where T : class
{
    // ── Reads ──────────────────────────────────────────────
    /// <summary>جيب record بالـ Id</summary>
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);

    Task<T?> GetAsync(Expression<Func<T, bool>> criteria,
                      CancellationToken ct = default);

    // ── Specification Reads ────────────────────────────────
    Task<T?> GetAsync(BaseSpecification<T> spec, CancellationToken ct = default);
    Task<List<T>> GetAllAsync(BaseSpecification<T> spec, CancellationToken ct = default);
    Task<int> CountAsync(BaseSpecification<T> spec, CancellationToken ct = default);

    /// <summary>جيب كل الـ records اللي تطابق الشرط</summary>
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? criteria = null,
                              CancellationToken ct = default);

    /// <summary>IQueryable للاستخدام مع ApplyFilters و ToPaginatedListAsync</summary>
    IQueryable<T> GetQueryable(Expression<Func<T, bool>>? criteria = null);

    /// <summary>هل فيه record يطابق الشرط؟</summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> criteria,
                        CancellationToken ct = default);

    /// <summary>عد الـ records</summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null,
                         CancellationToken ct = default);

    // ── Writes ─────────────────────────────────────────────
    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}