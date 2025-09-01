using System.ComponentModel.DataAnnotations.Schema;
using Application.DomainEvents.Events.Support;
using Domain.Common;

namespace Domain.Entities;
[Table("TicketMessages", Schema = "Ticket")]
public class SupportMessage : BaseEntity<long>
{
    public long TicketId { get; set; }
    public long SenderId { get; set; }         
    public bool IsFromAdmin { get; set; }       
    public string Text { get; set; } = "";
    public string? AttachmentPath { get; set; } 
    public bool IsReadByUser { get; set; } = false;
    public bool IsReadByAdmin { get; set; } = false;

    public SupportTicket? Ticket { get; set; } 
    
    public User Sender { get; set; }
    
    public void RaiseAddedEvent() => AddDomainEvent(new SupportMessageAddedEvent(this.TicketId, this.Id, this.IsFromAdmin));
    
    
}