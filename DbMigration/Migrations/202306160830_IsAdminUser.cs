using FluentMigrator;

namespace DbMigration.Migrations;

[Migration(202306160830)]
public class IsAdminUser : Migration
{
    public override void Up()
    {
        Alter.Table("users").AddColumn("is_admin").AsBoolean().NotNullable().WithDefaultValue(false);
    }

    public override void Down()
    {
        Delete.Column("isAdmin").FromTable("users");
    }
}