using FluentMigrator;

namespace DbMigration.Migrations;

[Migration(202304131811)]
public class InitNuggetTables : Migration
{
    public override void Up()
    {
        Execute.Script("InstallExtension.sql");

        Create.Table("Nugget")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().Unique().WithDefaultValue(SystemMethods.NewGuid)
            .WithColumn("Title").AsString().NotNullable()
            .WithColumn("Description").AsString().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().Indexed()
            .WithColumn("created_by").AsString().NotNullable().Indexed()
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().Indexed()
            .WithColumn("updated_by").AsString().NotNullable().Indexed();
    }

    public override void Down()
    {
        Delete.Table("Nugget");
    }
}