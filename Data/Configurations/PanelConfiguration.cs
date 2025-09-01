using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class PanelConfiguration : IEntityTypeConfiguration<Panel>
{
    public void Configure(EntityTypeBuilder<Panel> builder)
    {
        builder.ToTable("Panels",schema:"Commerce");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserName)
            .HasMaxLength(150)
            .HasColumnType("nvarchar(150)")
            .IsRequired();

        builder.Property(p=>p.Name)
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)")
            .HasDefaultValue("NO-PANEL-NAME")
            .IsRequired();
        
        builder.Property(p=>p.Url)
            .HasColumnType("nvarchar(250)")
            .HasMaxLength(250)
            .IsRequired();
        
        builder.HasOne(p=>p.User)
            .WithMany(u=>u.Panels)
            .HasForeignKey(p=>p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Password)
            .HasColumnType("nvarchar(250)")
            .HasMaxLength(250)
            .IsRequired();

       builder.HasMany(p=>p.Instances)
           .WithOne(i=>i.Panel)
           .HasForeignKey(i=>i.PanelId)
           .OnDelete(DeleteBehavior.Cascade);
       
       builder.Property(n=>n.CertificateKey)
           .HasColumnType("nvarchar(4000)")
           .IsRequired(false);
       
       builder.Property(p=>p.ApiPort)
           .HasColumnType("int")
           .IsUnicode()
           .IsRequired();
       
       builder.Property(p=>p.XrayPort)
           .HasColumnType("int")
           .IsUnicode()
           .IsRequired();
       
       builder.Property(p=>p.InboundPort)
           .HasColumnType("int")
           .IsUnicode()
           .IsRequired();
       
       builder.Property(i => i.IsDelete)
           .HasDefaultValue(false);

       builder.Property(p => p.SSL)
           .HasDefaultValue(true);

    }
}