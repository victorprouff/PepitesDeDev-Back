using System.Data;
using FluentMigrator;

namespace DbMigration.Migrations;

[Migration(202311011436)]
public class AddCommentTable : Migration
{
    public override void Up()
    {
        Create.Table("comments")
            .WithColumn("id").AsGuid().NotNullable().Unique().PrimaryKey().WithDefaultValue(SystemMethods.NewGuid)
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("nugget_id").AsGuid().NotNullable()
            .ForeignKey("FK_nuggets_id_comments_id", "nuggets", "id").OnDelete(Rule.Cascade)
            .WithColumn("content").AsString().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().Indexed()
            .WithColumn("updated_at").AsDateTimeOffset().Nullable().Indexed();
    }

    public override void Down()
    {
        Delete.Table("Comment");
    }
}