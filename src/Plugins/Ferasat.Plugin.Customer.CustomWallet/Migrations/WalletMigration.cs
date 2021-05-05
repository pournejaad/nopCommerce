using System;
using Ferasat.Plugin.Customer.CustomWallet.Models;
using FluentMigrator;
using Nop.Data.Migrations;

namespace Ferasat.Plugin.Customer.CustomWallet.Migrations
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/05/03 09:36:08:9037696")]
    public class WalletMigration : AutoReversingMigration
    {
        private IMigrationManager _migrationManager;

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