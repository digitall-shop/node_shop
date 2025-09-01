using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class InstanceConfiguration : IEntityTypeConfiguration<Instance>
{
    public void Configure(EntityTypeBuilder<Instance> builder)
    {
        builder.ToTable("Instance", schema: "Commerce");
        builder.HasKey(x => x.Id);

        builder.Property(c => c.ContainerDockerId)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(c => c.ProvisionedInstanceId)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(c => c.Node)
            .WithMany(ع => ع.Instances)
            .HasForeignKey(c => c.NodeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Instances)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Panel)
            .WithMany(p => p.Instances)
            .HasForeignKey(c => c.PanelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.ApiPort)
            .HasColumnType("int")
            .IsUnicode()
            .IsRequired();

        builder.Property(p => p.XrayPort)
            .HasColumnType("int")
            .IsUnicode()
            .IsRequired();

        builder.Property(p => p.InboundPort)
            .HasColumnType("int")
            .IsUnicode()
            .IsRequired();

        builder.Property(p => p.MarzbanNodeId)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(i => i.IsDelete)
            .HasDefaultValue(false);
        
        builder.Property(i => i.LastBilledUsage)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.LastBillingTimestamp)
            .IsRequired();
    }
}