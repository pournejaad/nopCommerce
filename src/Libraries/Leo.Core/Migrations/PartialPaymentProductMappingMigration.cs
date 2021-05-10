using FluentMigrator;
using Leo.Core.Payments;
using Nop.Data.Migrations;

namespace Leo.Core.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/05/06 10:00:00")]
    public class PartialPaymentProductMappingMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public PartialPaymentProductMappingMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<PartialPaymentProductMapping>(Create);
        }
    }
}