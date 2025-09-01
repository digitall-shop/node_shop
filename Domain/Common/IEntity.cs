namespace Domain.Common;

/// <summary>
/// Represents an entity with a primary key of type <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public interface IEntity<TKey>
{
    /// <summary>
    /// Primary key value.
    /// </summary>
    TKey Id { get; }
}