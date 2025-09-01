using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", schema: "Account");
        builder.HasKey(r => r.Id);

        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.IsSuperAdmin)
            .HasDefaultValue(false);

        builder.Property(u => u.UserName)
            .HasMaxLength(100)
            .IsRequired()
            .HasDefaultValue("NO-USERNAME");

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .HasDefaultValue("NO-NAME")
            .IsRequired(false);

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(u => u.Balance)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasMany(u => u.Panels)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Instances)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(u=>u.Transactions)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(u=>u.PaymentRequests)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.SupportTickets)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AssignedTickets)
            .WithOne(t => t.AssignedAdmin)
            .HasForeignKey(t => t.AssignedAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.SupportMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(u=>u.Credit)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0)
            .IsRequired();
        
        builder.Property(u=>u.PriceMultiplier)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(1)
            .IsRequired();
        
        builder.Property(u=>u.PaymentAccess)
            .HasConversion<string>()
            .HasMaxLength(50); 
        
        builder.Property(u=>u.LowBalanceNotified)
            .HasDefaultValue(false)
            .IsRequired(false);
    }
}