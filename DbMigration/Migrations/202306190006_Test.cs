using FluentMigrator;

namespace DbMigration.Migrations;

[Migration(202306190006)]
public class Test : Migration
{
    public override void Up()
    {
        Create.Table("test").WithColumn("id").AsGuid().NotNullable().PrimaryKey().Unique()
            .WithDefaultValue(SystemMethods.NewGuid);
    }

    public override void Down()
    {
        Delete.Table("test");
    }
}