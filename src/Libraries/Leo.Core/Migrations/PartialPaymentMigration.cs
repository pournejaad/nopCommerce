using FluentMigrator;
using Leo.Core.Payments;
using Nop.Data.Migrations;

namespace Leo.Core.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/05/05 12:30:00")]
    public class PartialPaymentMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public PartialPaymentMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<PartialPayment>(Create);
        }
    }
}