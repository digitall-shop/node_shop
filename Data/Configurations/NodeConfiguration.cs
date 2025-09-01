using Domain.Entities;
using Domain.Enumes.Node;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.ToTable("Node",schema:"Commerce");
        builder.HasKey(x => x.Id);

        builder.Property(n=>n.NodeName)
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        builder.Property(n => n.SshPort)
            .HasDefaultValue(22)
            .HasColumnType("int")
            .IsRequired();
        
        builder.Property(n=>n.SshHost)
            .HasColumnType("varchar(100)")
            .IsRequired();
        
        builder.Property(n=>n.SshUsername)
            .HasColumnType("varchar(200)")
            .IsRequired();
        
        builder.Property(n=>n.SshPassword)
            .HasColumnType("varchar(100)")
            .IsRequired();
        
        builder.Property(n=>n.Method)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        builder.Property(n => n.Price)
            .HasPrecision(18,2)
            .IsRequired();
        
        builder.Property(n => n.XrayContainerImage)
            .HasMaxLength(255)
            .IsRequired(false);
        
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(NodeStatus.Active)
            .IsRequired();
        
        builder.Property(n=>n.IsAvailableForShow)
            .HasColumnType("bit")
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.HasMany(n=>n.Instances)
            .WithOne(n => n.Node)
            .HasForeignKey(n=>n.NodeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(i => i.IsDelete)
            .HasDefaultValue(false);

        // New agent fields mapping
        builder.Property(n => n.IsEnabled)
            .HasColumnType("bit")
            .HasDefaultValue(true);
        builder.Property(n => n.ProvisioningStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(NodeProvisioningStatus.Pending);
        builder.Property(n => n.ProvisioningMessage)
            .HasMaxLength(1000);
        builder.Property(n => n.AgentVersion)
            .HasMaxLength(50);
        builder.Property(n => n.TargetAgentVersion)
            .HasMaxLength(50);
        builder.Property(n => n.InstallMethod)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(AgentInstallMethod.Docker);
        builder.Property(n => n.MarzbanEndpoint)
            .HasMaxLength(255);
        builder.Property(n => n.AgentEnrollmentToken)
            .HasMaxLength(255);
    }
}