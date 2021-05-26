using FluentMigrator.Builders.Create.Table;
using Leo.Core.Payments;
using Nop.Data.Mapping.Builders;

namespace Leo.Core.Mappings.Builders
{
    public class PartialPaymentUsageHistoryBuilder : NopEntityBuilder<PartialPaymentUsageHistory>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }
    }
}