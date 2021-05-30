using Leo.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nop.Data.Mapping.PartialPayments
{
    public class PartialPaymentUsageHistoryMap : NopEntityTypeConfiguration<PartialPaymentUsageHistory>
    {
        public override void Configure(EntityTypeBuilder<PartialPaymentUsageHistory> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedOn)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();
            builder.Property(e => e.SpentValue).HasColumnType("decimal(18,4)");
            base.Configure(builder);
        }
    }
}