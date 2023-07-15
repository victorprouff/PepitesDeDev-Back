using FluentMigrator;

namespace DbMigration.Migrations;

[Migration(202307092057)]
public class AddImageNugget : Migration
{
    public override void Up()
    {
        Alter.Table("nuggets").AddColumn("url_image").AsString().Nullable();
    }

    public override void Down()
    {
        Delete.Column("url_image").FromTable("nuggets");
    }
}