using System;
using FluentMigrator;
using Leo.Core.Customers;
using Nop.Data.Migrations;

namespace Leo.Core.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/05/03 09:36:08:9037696")]
    public class WalletMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public WalletMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
           _migrationManager.BuildTable<Wallet>(Create); 
        }
    }
}