using Leo.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Leo.Data
{
    public class CustomerWalletDepositHistoryMap : NopEntityTypeConfiguration<CustomerWalletDeposit>
    {
        public override void Configure(EntityTypeBuilder<CustomerWalletDeposit> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedOn)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();
            builder.Property(e => e.Amount).HasColumnType("decimal(18,4)");
            builder.Property(e => e.Source).HasMaxLength(256);
            base.Configure(builder);
        }
    }
}