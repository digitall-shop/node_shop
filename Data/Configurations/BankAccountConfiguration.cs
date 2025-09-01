using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("BankAccount",schema:"Account");
        builder.HasKey(x => x.Id);
        
        builder.Property(b=>b.IsDelete)
            .HasDefaultValue(false);

        builder.Property(b => b.CardNumber)
            .HasMaxLength(16)
            .IsUnicode()
            .IsRequired();
        
        builder.Property(b => b.BankName)
            .HasDefaultValue("nvarchar(50)")
            .IsRequired();
        
        builder.Property(b=>b.HolderName)
            .HasDefaultValue("nvarchar(100)")
            .IsRequired();
        
        builder.Property(b=>b.IsActive)
            .HasDefaultValue(true);
        
        builder.HasMany(b => b.PaymentRequests)
            .WithOne(p => p.BankAccount)
            .HasForeignKey(p => p.BankAccountId) 
            .OnDelete(DeleteBehavior.Restrict);
    }
}