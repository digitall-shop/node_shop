using Domain.Entities;
using Domain.Enumes.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class PaymentRequestConfiguration : IEntityTypeConfiguration<PaymentRequest>
{
    public void Configure(EntityTypeBuilder<PaymentRequest> builder)
    {
        builder.ToTable("PaymentRequests", schema: "Payment");
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasDefaultValue(PaymentRequestStatus.Pending)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.Method)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.ReceiptImageUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(p => p.Description)
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(p => p.GatewayTransactionId)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.HasIndex(p => p.GatewayTransactionId);

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.IsDelete)
            .HasDefaultValue(false);

        builder.HasOne(p => p.BankAccount)
            .WithMany(b => b.PaymentRequests)
            .HasForeignKey(p => p.BankAccountId)
            .IsRequired(false);
    }
}