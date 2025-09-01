using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class CurrencyRateConfiguration : IEntityTypeConfiguration<CurrencyRate>
{
    public void Configure(EntityTypeBuilder<CurrencyRate> builder)
    {
        builder.ToTable("CurrencyRates", schema: "Setting");
        builder.HasKey(x => x.Id);

        builder.Property(c => c.CurrencyCode)
            .HasMaxLength(5)
            .IsRequired()
            .IsUnicode();

        builder.Property(c => c.RateToBase);

        builder.Property(c => c.IsManual)
            .HasDefaultValue(true);
    }
}