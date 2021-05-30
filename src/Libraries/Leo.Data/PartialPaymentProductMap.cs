using Leo.Core.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Leo.Data
{
    public class PartialPaymentProductMap : NopEntityTypeConfiguration<PartialPaymentProductMapping>
    {
        public override void Configure(EntityTypeBuilder<PartialPaymentProductMapping> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.ProductId)
                .IsUnique();
            base.Configure(builder);
        }
    }
}