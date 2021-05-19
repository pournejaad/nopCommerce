using System.Data;
using FluentMigrator.Builders.Create.Table;
using Leo.Plugin.Payments.CustomerWallet.Models;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;

namespace Leo.Plugin.Payments.CustomerWallet.Mappings.Builders
{
    public class WalletBuilder : NopEntityBuilder<Wallet>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(Wallet.Id)).AsInt32().PrimaryKey()
                .WithColumn(nameof(Wallet.CustomerId)).AsInt32()
                .ForeignKey<Nop.Core.Domain.Customers.Customer>(onDelete:Rule.Cascade)
                .WithColumn(nameof(Wallet.Balance)).AsDecimal();
        }
    }
}