using FluentMigrator;

namespace DbMigration.Migrations;

[Migration(202309131352)]
public class AddIsEnabledNugget : Migration
{
    public override void Up()
    {
        Create.Column("is_enabled").OnTable("nuggets").AsBoolean().NotNullable().WithDefaultValue(true);
    }

    public override void Down()
    {
        Delete.Column("is_enabled").FromTable("nuggets");
    }
}