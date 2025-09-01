using System.Linq.Expressions;
using Domain.Enumes.Paging;

namespace Domain.Common;


/// <summary>
/// Base DTO for paging and filtering.
/// Each entity-specific DTO inherits from this class
/// and implements <see cref="BuildPredicate"/> to create its own filter.
/// </summary>
public abstract class Paging<TDto>
{
    // Default values
    private const int DefaultSkip = 0;
    private const int DefaultTake = 20;

    /// <summary>How many records to skip (for paging).</summary>
    public int Skip { get; set; } = DefaultSkip;

    /// <summary>How many records to take (page size).</summary>
    public int Take { get; set; } = DefaultTake;

    /// <summary>
    ///     Build the LINQ predicate that will be applied to <typeparamref name="TDto"/>.
    /// </summary>
    public abstract Expression<Func<TDto, bool>> BuildPredicate();
}

public abstract class PagedDto
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}

public sealed class BasePaging<T>
{
    public int Page       { get; set; }
    public int Take       { get; set; }
    public int TotalCount { get; set; }
    public List<T> Items  { get; set; } = [];
}

public sealed class FilterCriterion
{
    public required string PropertyName { get; init; }
    public required FilterOperator Operator { get; init; }
    public required object Value { get; init; }
}