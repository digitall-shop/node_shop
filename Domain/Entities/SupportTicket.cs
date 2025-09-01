using System.ComponentModel.DataAnnotations.Schema;
using Application.DomainEvents.Events.Support;
using Domain.Common;
using Domain.Enumes.Suppotr;

namespace Domain.Entities;
[Table("SupportTickets", Schema = "Ticket")]
public class SupportTicket : BaseEntity<long> 
{
  
    public long UserId { get; set; }               
    public string? Subject { get; set; } 
    public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;
    public long? AssignedAdminId { get; set; }    
    public DateTime? ClosedAt { get; set; }
    public User? AssignedAdmin { get; set; } 
    public User? User { get; set; }  
    public ICollection<SupportMessage> Messages { get; set; }
    public void RaiseCreated() => AddDomainEvent(new SupportTicketCreatedEvent(this.Id));
}