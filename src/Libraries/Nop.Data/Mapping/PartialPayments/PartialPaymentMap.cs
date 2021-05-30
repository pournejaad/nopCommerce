using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Leo.Core.Domain;

namespace Nop.Data.Mapping.PartialPayments
{
    public class PartialPaymentMap : NopEntityTypeConfiguration<Leo.Core.Domain.PartialPayment>
    {
        public override void Configure(EntityTypeBuilder<Leo.Core.Domain.PartialPayment> builder)
        {
            builder.ToTable(nameof(Leo.Core.Domain.PartialPayment));
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.Name).HasMaxLength(512).IsRequired();
            builder.Property(e => e.MaximumPartialPaymentAmount).HasColumnType("decimal(18,4)");

            builder.Property(entity => entity.PartialPaymentAmount).HasColumnType("decimal(18,4)");
            builder.Property(entity => entity.MaximumPartialPaymentAmount)
                .HasColumnType("decimal(18,4)");
            base.Configure(builder);
        }
    }
}
