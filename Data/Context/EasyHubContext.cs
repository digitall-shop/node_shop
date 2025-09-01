using System.Reflection;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

public class NodeShopContext(IMediator mediator,DbContextOptions<NodeShopContext> options) : DbContext(options)
{
    DbSet<User> Users { get; set; }
    DbSet<Node> Nodes { get; set; }
    DbSet<Panel> Panels { get; set; }
    DbSet<Instance> Instances { get; set; }
    DbSet<BankAccount> BankAccounts { get; set; }
    DbSet<Transaction> Transactions { get; set; }
    DbSet<PaymentRequest> PaymentRequests { get; set; }
    DbSet<Log> Logs { get; set; }
    DbSet<SupportTicket> SupportTickets { get; set; } 
    DbSet<SupportMessage> SupportMessages { get; set;}

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(x => x.ClearDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply any other IEntityTypeConfiguration<> in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}