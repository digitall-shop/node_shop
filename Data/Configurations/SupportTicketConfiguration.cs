using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("SupportTickets",schema: "Ticket");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Subject)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.CreateDate)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.ModifiedDate)
            .HasColumnType("datetime2");

        builder.Property(x => x.ClosedAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.IsDelete)
            .HasDefaultValue(false);

        builder.HasOne(t => t.User)                     
            .WithMany(u => u.SupportTickets)          
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.AssignedAdmin)
            .WithMany(u => u.AssignedTickets) 
            .HasForeignKey(x => x.AssignedAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => x.AssignedAdminId);
        builder.HasIndex(x => x.CreateDate);
        
        builder.HasMany(x=>x.Messages)
            .WithOne(x=>x.Ticket)
            .HasForeignKey(x=>x.TicketId)
            .OnDelete(DeleteBehavior.Restrict);
        
    }
}