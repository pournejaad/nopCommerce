using FluentMigrator.Builders.Create.Table;
using Leo.Core.Payments;
using Nop.Data.Mapping.Builders;

namespace Leo.Core.Mappings.Builders
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