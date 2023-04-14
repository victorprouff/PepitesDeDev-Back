using FluentMigrator;

namespace DbMigration.Migrations;

[Migration(202304141820)]
public class AddUserTable : Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().Unique().WithDefaultValue(SystemMethods.NewGuid)
            .WithColumn("email").AsString().NotNullable()
            .WithColumn("password").AsString().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().Indexed()
            .WithColumn("updated_at").AsDateTimeOffset().Nullable().Indexed();
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}