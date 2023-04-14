using FluentMigrator;

namespace DbMigration.Migrations;

[Migration(202304131811)]
public class InitNuggetTable : Migration
{
    public override void Up()
    {
        Execute.Script("InstallExtension.sql");

        Create.Table("nuggets")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().Unique().WithDefaultValue(SystemMethods.NewGuid)
            .WithColumn("title").AsString().NotNullable()
            .WithColumn("description").AsString().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().Indexed()
            .WithColumn("updated_at").AsDateTimeOffset().Nullable().Indexed();
    }

    public override void Down()
    {
        Delete.Table("nuggets");
    }
}