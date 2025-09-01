using System.Linq.Expressions;
using Data.Context;
using Domain.Common;
using Domain.Contract;
using Domain.Entities;
using Domain.Enumes.Paging;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class BaseRepository<TEntity, TKey>(NodeShopContext context)
    : IAsyncRepository<TEntity, TKey>,IDisposable,IAsyncDisposable where TEntity : class, IEntity<TKey>
{
    public async ValueTask DisposeAsync() => await context.DisposeAsync();
    
    public IQueryable<TEntity> GetQuery()
    {
        return context.Set<TEntity>().AsQueryable();
    }
    
    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return await context.Set<TEntity>()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string? includeString = null,
        bool disableTracking = true)
    {
        var query = GetQuery();

        if (disableTracking) query = query.AsNoTracking();

        if (includeString != null) query = query.Include(includeString);

        if (orderBy is not null) return await orderBy(query).ToListAsync();

        return await query.ToListAsync();
    }
    

    public async Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null, bool disableTracking = true)
    {
        var query = GetQuery();

        if (disableTracking) query = query.AsNoTracking();

        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (predicate != null) query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).ToListAsync();

        return await query.ToListAsync();
    }

    public async Task<long> CountAsync()
    {
        var query = GetQuery();
        return await query.LongCountAsync();
    }

    public Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var query = GetQuery();
        return query.LongCountAsync(predicate);
    }

    public async Task<TEntity> AddEntity(TEntity entity)
    {
        await context.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddEntities(IEnumerable<TEntity> entities)
    {
        await context.Set<TEntity>().AddRangeAsync(entities);
        return entities;
    }

    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await GetSingleAsync(predicate,includeString : null);
    }

    public async Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> predicate,
        string? includeString = null,
        bool disableTracking = true
    )
    {
        IQueryable<TEntity> query = GetQuery();

        if (disableTracking)
            query = query.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(includeString))
            query = query.Include(includeString);

        // SingleOrDefaultAsync will return null if nothing matches
        return await query.SingleOrDefaultAsync(predicate);
    }

    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, List<Expression<Func<TEntity, object>>>? includes = null, bool disableTracking = true)
    {
        IQueryable<TEntity> query = GetQuery();

        if (disableTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        // SingleOrDefaultAsync will return null if nothing matches
        return await query.SingleOrDefaultAsync(predicate);
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await context.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity?> GetByIdAsync(
        TKey id,
        string includeString = null,
        bool disableTracking = true)
    {
        IQueryable<TEntity> query = GetQuery();

        if (disableTracking)
            query = query.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(includeString))
            query = query.Include(includeString);

        return await query.SingleOrDefaultAsync(e => e.Id.Equals(id));
    }

    public async Task<TEntity?> GetByIdAsync(
        TKey id,
        List<Expression<Func<TEntity, object>>> includes = null,
        bool disableTracking = true)
    {
        IQueryable<TEntity> query = GetQuery();

        if (disableTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.SingleOrDefaultAsync(e => e.Id.Equals(id));
    }

    public async Task UpdateEntity(TEntity entity)
    {
        context.Set<TEntity>().Entry(entity).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public Task UpdateEntities(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            context.Set<TEntity>().Entry(entity).State = EntityState.Modified;
        }

        return Task.CompletedTask;
    }

    public void DeleteEntity(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
    }

    public async Task DeleteEntity(TKey entityId)
    {
        TEntity? entity = await GetByIdAsync(entityId);
        if (entity is not null) DeleteEntity(entity);
    }

    public void DeletePermanent(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task DeletePermanent(long entityId)
    {
        throw new NotImplementedException();
    }

    public Task Deletes(List<TEntity> entities)
    {
        throw new NotImplementedException();
    }


    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await context.Set<TEntity>().AnyAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, string includeString)
    {
        var query = GetQuery();

        if (!string.IsNullOrWhiteSpace(includeString))
            query = query.Include(includeString);

        return await query.AnyAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,
        List<Expression<Func<TEntity, object>>>? includes)
    {
        var query = GetQuery();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.AnyAsync(predicate);
    }
    

    public async Task<BasePaging<TEntity>> GetPagedDataAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        List<FilterCriterion>? filters = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        int page = 1,
        int take = 20)
    {
        IQueryable<TEntity> q = GetQuery();

        if (predicate != null)
            q = q.Where(predicate);

        if (filters?.Any() == true)
        {
            var p = Expression.Parameter(typeof(TEntity), "x");
            Expression? body = null;

            foreach (var f in filters)
            {
                Expression bin;

                if (f.PropertyName.Contains('.'))
                {
                    var props = f.PropertyName.Split('.');
                    Expression inner = Expression.PropertyOrField(p, props[0]);

                    for (int i = 1; i < props.Length - 1; i++)
                        inner = Expression.PropertyOrField(inner, props[i]);

                    var elementType = inner.Type.GetGenericArguments().First();
                    var paramY = Expression.Parameter(elementType, "y");
                    var lastProp = Expression.PropertyOrField(paramY, props[^1]);
                    var right = Expression.Constant(Convert.ChangeType(f.Value, lastProp.Type));

                    Expression condition = f.Operator switch
                    {
                        FilterOperator.Equals => Expression.Equal(lastProp, right),
                        FilterOperator.GreaterThan => Expression.GreaterThan(lastProp, right),
                        FilterOperator.LessThan => Expression.LessThan(lastProp, right),
                        FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(lastProp, right),
                        FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(lastProp, right),
                        FilterOperator.Contains => Expression.Call(
                            lastProp,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                            right),
                        _ => throw new NotSupportedException("Nested filter operator not supported.")
                    };

                    var lambda = Expression.Lambda(condition, paramY);
                    bin = Expression.Call(typeof(Enumerable), "Any", new[] { elementType }, inner, lambda);
                }
                else
                {
                    var left = Expression.PropertyOrField(p, f.PropertyName);
                    var right = Expression.Constant(Convert.ChangeType(f.Value, left.Type));

                    bin = f.Operator switch
                    {
                        FilterOperator.Equals => Expression.Equal(left, right),
                        FilterOperator.Contains => Expression.Call(
                            left,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                            right),
                        FilterOperator.GreaterThan => Expression.GreaterThan(left, right),
                        FilterOperator.LessThan => Expression.LessThan(left, right),
                        FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
                        FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(left, right),
                        _ => throw new NotSupportedException()
                    };
                }

                body = body == null ? bin : Expression.AndAlso(body, bin);
            }

            if (body != null)
            {
                var lambda = Expression.Lambda<Func<TEntity, bool>>(body, p);
                q = q.Where(lambda);
            }
        }


        if (includes != null)
            includes.ForEach(include => q = q.Include(include));

        q = orderBy?.Invoke(q) ?? q;

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * take).Take(take).ToListAsync();

        return new BasePaging<TEntity>
        {
            Page = page,
            Take = take,
            TotalCount = total,
            Items = items
        };
    }


    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}