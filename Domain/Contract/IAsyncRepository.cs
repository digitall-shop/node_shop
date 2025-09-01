using System.Linq.Expressions;
using Domain.Common;
using Domain.Entities;

namespace Domain.Contract;

/// <summary>
/// Defines a generic asynchronous repository for entities with manually assigned primary keys.
/// </summary>
/// <typeparam name="TEntity">
/// </typeparam>
/// <typeparam name="TKey">Type of the entity’s primary key.</typeparam>
public interface IAsyncRepository<TEntity, in TKey> : IAsyncDisposable
    where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// Returns an <see cref="IQueryable{TEntity}"/> for further querying.
    /// </summary>
    /// If true, returns only deleted entities; if false, only non-deleted; if null, returns all.
    IQueryable<TEntity> GetQuery();

    public void Dispose();

    /// <summary>
    /// Retrieves all entities as a read-only list.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync();

    /// <summary>
    /// Retrieves all entities matching the given filter.
    /// </summary>
    /// <param name="predicate">A filter expression.</param>
    Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Retrieves entities with optional filtering, ordering, and simple include string.
    /// </summary>
    /// <param name="predicate">Optional filter expression.</param>
    /// <param name="orderBy">Optional ordering function.</param>
    /// <param name="includeString">Optional include path for related entities.</param>
    /// <param name="disableTracking">
    /// If true, the query is executed without EF change tracking.
    /// </param>
    Task<IReadOnlyList<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string? includeString = null,
        bool disableTracking = true);

    /// <summary>
    /// Retrieves entities with optional filtering, ordering, and multiple include expressions.
    /// </summary>
    /// <param name="predicate">Optional filter expression.</param>
    /// <param name="orderBy">Optional ordering function.</param>
    /// <param name="includes">
    /// Optional list of include expressions for related entities.
    /// </param>
    /// <param name="disableTracking">
    /// If true, the query is executed without EF change tracking.
    /// </param>
    Task<IReadOnlyList<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true);

    /// <summary>
    /// Get count all entity in db
    /// </summary>
    /// <returns></returns>
    Task<long> CountAsync();

    /// <summary>
    /// get count all from db by condition
    /// </summary>
    /// <param name="predicate">Optional filter expression.</param>
    /// <returns></returns>
    Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    Task<TEntity> AddEntity(TEntity entity);

    /// <summary>
    /// Adds multiple entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    Task<IEnumerable<TEntity>> AddEntities(IEnumerable<TEntity> entities);

    /// <summary>
    /// Retrieves a single entity matching the given predicate, optionally including related navigation properties.
    /// Returns null if no match is found.
    /// </summary>
    /// <param name="predicate">Filter condition to identify the entity.</param>
    /// If true, the query will use AsNoTracking() for read-only scenarios.
    Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> predicate
    );
    
    /// <summary>
    /// Retrieves a single entity matching the given predicate, optionally including related navigation properties.
    /// Returns null if no match is found.
    /// </summary>
    /// <param name="predicate">Filter condition to identify the entity.</param>
    /// <param name="includeString">
    /// A dot-separated include path (e.g. "UserRoles.Role") 
    /// to eager-load related entities. Optional.
    /// </param>
    /// <param name="disableTracking">
    /// If true, the query will use AsNoTracking() for read-only scenarios.
    /// </param>
    Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> predicate,
        string? includeString = null,
        bool disableTracking = true
    );
    
    /// <summary>
    /// Retrieves a single entity matching the given predicate, optionally including related navigation properties.
    /// Returns null if no match is found.
    /// </summary>
    /// <param name="predicate">Filter condition to identify the entity.</param>
    /// <param name="includes">
    /// A dot-separated include path (e.g. "UserRoles.Role") 
    /// to eager-load related entities. Optional.
    /// </param>
    /// <param name="disableTracking">
    /// If true, the query will use AsNoTracking() for read-only scenarios.
    /// </param>
    Task<TEntity?> GetSingleAsync(
        Expression<Func<TEntity, bool>> predicate,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true
    );
    
    /// <summary>
    /// Retrieves a single entity by its key.
    /// </summary>
    Task<TEntity?> GetByIdAsync(TKey id);

    /// <summary>
    /// Retrieves a single entity by its key, with optional include path (string).
    /// </summary>
    Task<TEntity?> GetByIdAsync(
        TKey id,
        string includeString,
        bool disableTracking = true);

    /// <summary>
    /// Retrieves a single entity by its key, with optional include expressions.
    /// </summary>
    Task<TEntity?> GetByIdAsync(
        TKey id,
        List<Expression<Func<TEntity, object>>> includes,
        bool disableTracking = true);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    Task UpdateEntity(TEntity entity);

    /// <summary>
    /// Updates a range of entities in the data store.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    Task UpdateEntities(IEnumerable<TEntity> entities);

    /// <summary>
    /// Soft-deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void DeleteEntity(TEntity entity);

    /// <summary>
    /// Soft-deletes an entity by its primary key.
    /// </summary>
    /// <param name="entityId">The primary key of the entity to delete.</param>
    Task DeleteEntity(TKey entityId);

    /// <summary>
    /// Permanently removes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void DeletePermanent(TEntity entity);

    /// <summary>
    /// Permanently removes an entity by its primary key.
    /// </summary>
    /// <param name="entityId">The primary key of the entity to remove.</param>
    Task DeletePermanent(long entityId);

    /// <summary>
    /// Soft-deletes a list of entities.
    /// </summary>
    /// <param name="entities">The entities to delete.</param>
    Task Deletes(List<TEntity> entities);

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    /// Determines whether any entities exist that match the specified condition.
    /// </summary>
    /// <param name="predicate"></param>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Determines whether any entities exist that match the specified condition, including a related entity using an include string.
    /// </summary>
    /// <param name="predicate">A filter expression to apply to the entity set.</param>
    /// <param name="includeString">A dot-separated include path for related entities (e.g., "Orders.Items").</param>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, string includeString);

    /// <summary>
    /// Determines whether any entities exist that match the specified condition, including one or more related entities.
    /// </summary>
    /// <param name="predicate">A filter expression to apply to the entity set.</param>
    /// <param name="includes">A list of expressions specifying the related entities to include (e.g., x => x.Orders).</param>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, List<Expression<Func<TEntity, object>>>? includes);
    
        /// <summary>
    /// Asynchronously returns a single page of <typeparamref name="TEntity"/> records,
    /// with optional filtering, eager-loading, and custom ordering.
    /// </summary>
    /// <param name="predicate">A filter expression to apply to the entity set.</param>
    /// <param name="filters">
    ///     A LINQ expression used to filter the entity set (the <c>WHERE</c> clause).
    ///     Pass <c>null</c> to fetch all rows.
    /// </param>
    /// <param name="orderBy">
    ///     A delegate that applies the desired sort order
    ///     (e.g. <c>q =&gt; q.OrderByDescending(x =&gt; x.CreatedAt)</c>).
    ///     If <c>null</c>, the natural order of the underlying query is used.
    /// </param>
    /// <param name="includes">
    ///     A list of navigation-property expressions to eager-load
    ///     (translated to EF Core <c>Include</c>/<c>ThenInclude</c> calls).
    ///     Use <c>null</c> or an empty list to skip eager loading.
    /// </param>
    /// <param name="page">
    ///     1-based page number to fetch. Values &lt; 1 are coerced to 1.
    /// </param>
    /// <param name="take">
    ///     Maximum number of items per page (page size). Values &lt;= 0 default to 10.
    /// </param>
    /// <returns>
    ///     A <see cref="BasePaging{T}"/> instance containing:
    ///     <list type="bullet">
    ///         <item><description><c>Items</c> – the current page of entities</description></item>
    ///         <item><description><c>TotalCount</c> – total rows before paging</description></item>
    ///         <item><description><c>Page</c> / <c>Take</c> – echo of the requested paging parameters</description></item>
    ///     </list>
    /// </returns>
    Task<BasePaging<TEntity>> GetPagedDataAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        List<FilterCriterion>? filters = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        int page = 1,
        int take = 20);
    

}