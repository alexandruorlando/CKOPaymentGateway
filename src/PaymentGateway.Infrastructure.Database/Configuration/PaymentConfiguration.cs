using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;

namespace PaymentGateway.Infrastructure.Database.Configuration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),
                v => (PaymentStatus) Enum.Parse(typeof(PaymentStatus), v)
            );
    }
}