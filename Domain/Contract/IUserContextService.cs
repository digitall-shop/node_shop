namespace Domain.Contract;

public interface IUserContextService
{
    /// <summary>
    /// Gets the current authenticated user's Id from the JWT token.
    /// Returns 0 if not authenticated or claim not present.
    /// </summary>
    long UserId { get; }
    
}