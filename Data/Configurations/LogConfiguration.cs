using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class LogConfiguration :IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("Logs", schema: "System");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Message)
            .IsRequired(); 
        builder.Property(l => l.MessageTemplate)
            .IsRequired(false);

        builder.Property(l => l.Level)
            .IsRequired()
            .HasMaxLength(50); 

        builder.Property(l => l.Timestamp)
            .IsRequired();

        builder.Property(l => l.Exception)
            .IsRequired(false); 

        builder.Property(l => l.Properties)
            .IsRequired(false);
    }
}