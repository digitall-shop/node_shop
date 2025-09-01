using System.Linq.Expressions;
using Domain.Contract;
using Hangfire;


namespace Application.Infrastructure;

public class HangfireJobScheduler(
    IBackgroundJobClient backgroundJobClient,
    IRecurringJobManager recurringJobManager)
    : IJobScheduler
{
    public string Enqueue(Expression<Action> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }

    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        return backgroundJobClient.Enqueue(methodCall);
    }

    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
    {
        return backgroundJobClient.Schedule(methodCall, delay);
    }

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        return backgroundJobClient.Schedule(methodCall, delay);
    }

    public void AddOrUpdate(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression)
    {
        recurringJobManager.AddOrUpdate(recurringJobId, methodCall, cronExpression);
    }
}