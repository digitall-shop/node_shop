using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
{
    public void Configure(EntityTypeBuilder<SupportMessage> builder)
    {
        builder.ToTable("SupportMessages", schema:"Support");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(x => x.AttachmentPath)
            .HasMaxLength(500);

        builder.Property(x => x.CreateDate)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.IsReadByUser)
            .HasDefaultValue(false);

        builder.Property(x => x.IsReadByAdmin)
            .HasDefaultValue(false);

        builder.HasOne(m => m.Ticket)
            .WithMany(t => t.Messages)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Sender)
            .WithMany(u => u.SupportMessages)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TicketId, x.CreateDate });
    }
}