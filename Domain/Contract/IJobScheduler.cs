using System.Linq.Expressions;

namespace Domain.Contract;

/// <summary>
/// An abstraction for scheduling and executing background jobs.
/// The Application layer uses this interface to enqueue jobs
/// without knowing the actual implementation (e.g., Hangfire).
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Enqueues a job for immediate execution.
    /// </summary>
    /// <param name="methodCall">The method to be executed.</param>
    /// <returns>The ID of the created job.</returns>
    string Enqueue(Expression<Action> methodCall);

    /// <summary>
    /// Enqueues an asynchronous job for immediate execution.
    /// </summary>
    /// <param name="methodCall">The async method to be executed.</param>
    /// <returns>The ID of the created job.</returns>
    string Enqueue(Expression<Func<Task>> methodCall);

    /// <summary>
    /// Schedules a job to be executed after a specified delay.
    /// </summary>
    /// <param name="methodCall">The method to be executed.</param>
    /// <param name="delay">The delay duration.</param>
    /// <returns>The ID of the created job.</returns>
    string Schedule(Expression<Action> methodCall, TimeSpan delay);

    /// <summary>
    /// Schedules an asynchronous job to be executed after a specified delay.
    /// </summary>
    /// <param name="methodCall">The async method to be executed.</param>
    /// <param name="delay">The delay duration.</param>
    /// <returns>The ID of the created job.</returns>
    string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);

    // --- Implementation of a new method for recurring jobs ---
    /// <summary>
    /// Adds or updates a recurring job based on a CRON expression.
    /// </summary>
    /// <param name="recurringJobId">A unique identifier for the job</param>
    /// <param name="methodCall">The method to be executed</param>
    /// <param name="cronExpression">The CRON expression for scheduling</param>
    public void AddOrUpdate(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression);
}