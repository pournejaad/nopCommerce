using Leo.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Leo.Data
{
    public class PartialPaymentMap : NopEntityTypeConfiguration<PartialPayment>
    {
        public override void Configure(EntityTypeBuilder<PartialPayment> builder)
        {
            builder.ToTable(nameof(PartialPayment));
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
