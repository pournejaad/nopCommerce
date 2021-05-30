using Leo.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Leo.Data
{
    public class WalletMap : NopEntityTypeConfiguration<Wallet>
    {
        public override void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Balance).HasColumnType("decimal(18,4)");

            base.Configure(builder);
        }
    }
}