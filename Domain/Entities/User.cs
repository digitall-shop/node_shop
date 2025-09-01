using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Enumes.User;
using Domain.Events.DomainEvents.Events;

namespace Domain.Entities;

[Table("Users", Schema = "Account")]
public class User : BaseEntity<long>
{
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public bool IsSuperAdmin { get; set; }
    public bool? LowBalanceNotified { get; set; } = false;
    public PaymentMethodAccess PaymentAccess { get; set; }
    public bool IsBlocked { get; set; }
    public string? LastName { get; set; } 
    public long Balance { get; set; }
    public decimal PriceMultiplier { get; set; }
    public decimal Credit { get; set; }
    public string? Password { get; set; }

    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    public ICollection<Panel> Panels { get; set; }
    public ICollection<Instance> Instances { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
    public ICollection<PaymentRequest> PaymentRequests { get; set; }
    public ICollection<SupportTicket> SupportTickets { get; set; }
    public ICollection<SupportTicket> AssignedTickets { get; set; }
    public ICollection<SupportMessage> SupportMessages { get; set; }
    
    
    public void RaiseRegistered() => AddDomainEvent(new UserRegisteredEvent(this.Id));
    
}