using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions",schema:"Payment");
        builder.HasKey(t => t.Id);
        
        builder.HasOne(t=>t.PaymentRequest)
            .WithMany()
            .HasForeignKey(t => t.PaymentRequestId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(t=>t.User)
            .WithMany(t => t.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(t=>t.IsDelete)
            .HasDefaultValue(false);
        
        builder.Property(t=>t.Description)
            .HasColumnType("nvarchar(500)")
            .IsRequired(false);
        
        
        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("int");
        
        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(50); 
        
        builder.Property(t => t.Reason)
            .HasConversion<string>()
            .HasMaxLength(50);

    }
}