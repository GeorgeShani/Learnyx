using learnyx.Data;
using static System.GC;
using learnyx.Models.Common;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using learnyx.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace learnyx.Repositories.Implementation;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DataContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly ILogger<Repository<T>> _logger;

    public Repository(
        DataContext context, 
        ILogger<Repository<T>> logger
    ) {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public virtual IQueryable<T> QueryWithIncludes(params Expression<Func<T, object>>[] includes)
    {
        return includes.Aggregate<Expression<Func<T, object>>?, IQueryable<T>>(_dbSet, (current, include) => current.Include(include!));
    }

    public virtual IQueryable<T> QueryWithIncludes(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        return QueryWithIncludes(includes).Where(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all entities of type {EntityType}", typeof(T).Name);
            return await _dbSet.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            
            // Handle composite keys
            if (id is object[] keyValues)
            {
                return (await _dbSet.FindAsync(keyValues, cancellationToken))!;
            }
            
            return (await _dbSet.FindAsync(new[] { id }, cancellationToken))!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<T> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes)
    {
        try
        {
            _logger.LogDebug("Getting entity of type {EntityType} with ID {Id} and includes", typeof(T).Name, id);
            
            var entity = await GetByIdAsync(id);
            if (includes.Length != 0)
            {
                foreach (var include in includes)
                {
                    await _context.Entry(entity)
                        .Reference(include!)
                        .LoadAsync();
                }
            }
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity of type {EntityType} with ID {Id} and includes", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting first entity of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting first entity of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting single entity of type {EntityType} with predicate", typeof(T).Name);
            return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting single entity of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting paged entities of type {EntityType}, page {PageNumber}, size {PageSize}", 
                typeof(T).Name, pageNumber, pageSize);

            var totalCount = await _dbSet.CountAsync(cancellationToken);
            var items = await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting filtered paged entities of type {EntityType}, page {PageNumber}, size {PageSize}", 
                typeof(T).Name, pageNumber, pageSize);

            var query = _dbSet.Where(predicate);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Data = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting filtered paged entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }
    
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting filtered entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<long> LongCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.LongCountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error long counting entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<long> LongCountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.LongCountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error long counting filtered entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if any entity exists of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating entity of type {EntityType}", typeof(T).Name);
            
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Creating multiple entities of type {EntityType}", typeof(T).Name);
            
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            _dbSet.AddRange(entityList);
            await _context.SaveChangesAsync(cancellationToken);
            return entityList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating multiple entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating entity of type {EntityType}", typeof(T).Name);
            
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating multiple entities of type {EntityType}", typeof(T).Name);

            ArgumentNullException.ThrowIfNull(entities);

            var entityList = entities.ToList();
            _dbSet.UpdateRange(entityList);
            await _context.SaveChangesAsync(cancellationToken);
            return entityList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating multiple entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting entity of type {EntityType}", typeof(T).Name);

            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> DeleteByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            
            var entity = await GetByIdAsync(id, cancellationToken);
            return await DeleteAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting multiple entities of type {EntityType}", typeof(T).Name);

            ArgumentNullException.ThrowIfNull(entities);

            var entityList = entities.ToList();
            _dbSet.RemoveRange(entityList);
            await _context.SaveChangesAsync(cancellationToken);
            return entityList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting multiple entities of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting entities of type {EntityType} with predicate", typeof(T).Name);
            
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
            _dbSet.RemoveRange(entities);
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entities of type {EntityType} with predicate", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.MaxAsync(selector, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting max value of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.MinAsync(selector, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting min value of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.SumAsync(selector, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sum value of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<double> AverageAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.AverageAsync(selector, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting average value of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes");
            throw;
        }
    }
}