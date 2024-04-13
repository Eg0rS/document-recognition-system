using FluentMigrator;
using FluentMigrator.Snowflake;

namespace pdd_backend.Migrations;

[Migration(1)]
public class M0000_InitialMigration: Migration
{
    public override void Up()
    {
        Create.Table("requests")
            .WithColumn("id").AsInt64().Identity().NotNullable().PrimaryKey()
            .WithColumn("user_id").AsString().NotNullable()
            .WithColumn("guid").AsString().NotNullable()
            .WithColumn("file_id").AsString().NotNullable();
        Create.Table("resolutions")
            .WithColumn("id").AsInt64().Identity().NotNullable().PrimaryKey()
            .WithColumn("guid").AsString().NotNullable()
            .WithColumn("file_id").AsString().NotNullable()
            .WithColumn("data").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("requests");
        Delete.Table("resolutions");
    }
}
