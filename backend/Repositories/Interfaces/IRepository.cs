using System.Linq.Expressions;
using learnyx.Models.Common;

namespace learnyx.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    IQueryable<T> Query(Expression<Func<T, bool>> predicate);
    IQueryable<T> QueryWithIncludes(params Expression<Func<T, object>>[] includes);
    IQueryable<T> QueryWithIncludes(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    // Get methods
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<T> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Pagination
    Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    // Count methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<long> LongCountAsync(CancellationToken cancellationToken = default);
    Task<long> LongCountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Existence check
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    // CRUD operations
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<T> DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> DeleteByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Advanced operations
    Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
    Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
    Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default);
    Task<double> AverageAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default);
}