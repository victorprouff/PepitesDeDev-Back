using FluentMigrator;

namespace DbMigration.Migrations;

[Maintenance(MigrationStage.AfterAll, TransactionBehavior.None)]
public class GrantAccessMaintenance : Migration
{
    public override void Up()
    {
        Execute.Script("GrantAccess.sql");
    }

    public override void Down()
    {
        Execute.Script("GrantAccess.sql");
    }
}