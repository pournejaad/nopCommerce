using FluentMigrator.Builders.Create.Table;
using Leo.Plugin.Payments.CustomerWallet.Models;
using Nop.Data.Mapping.Builders;

namespace Leo.Plugin.Payments.CustomerWallet.Mappings.Builders
{
    public class PartialPaymentBuilder : NopEntityBuilder<PartialPayment>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PartialPayment.Name)).AsString(200).NotNullable();
        }
    }
}