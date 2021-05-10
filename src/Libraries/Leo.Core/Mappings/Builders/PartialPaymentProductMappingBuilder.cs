using System.Data;
using FluentMigrator.Builders.Create.Table;
using Leo.Core.Payments;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;

namespace Leo.Core.Mappings.Builders
{
    public class PartialPaymentProductMappingBuilder : NopEntityBuilder<PartialPaymentProductMapping>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PartialPaymentProductMapping.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(PartialPaymentProductMapping.PartialPaymentId)).AsInt32()
                .ForeignKey<PartialPayment>(onDelete: Rule.Cascade)
                .WithColumn(nameof(PartialPaymentProductMapping.ProductId)).AsInt32()
                .ForeignKey<Product>(onDelete: Rule.Cascade);
        }
    }
}