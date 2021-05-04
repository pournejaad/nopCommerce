using System.Data;
using System.Runtime.Serialization;
using Ferasat.Plugin.Customer.CustomWallet.Models;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;

namespace Ferasat.Plugin.Customer.CustomWallet.Mapping.Builders
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