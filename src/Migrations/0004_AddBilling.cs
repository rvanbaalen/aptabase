using FluentMigrator;

namespace Aptabase.Migrations;

[Migration(0004)]
public class AddBilling : Migration
{
    public override void Up()
    {
        Create.Table("billing")
            .WithNanoIdColumn("owner_id").PrimaryKey()
            .WithColumn("customer_id").AsInt64()
            .WithColumn("product_id").AsInt64()
            .WithColumn("variant_id").AsInt64()
            .WithColumn("subscription_id").AsInt64()
            .WithColumn("card_brand").AsString(20)
            .WithColumn("card_last_four").AsString(4)
            .WithColumn("manage_payment_url").AsCustom("TEXT")
            .WithTimestamps();
    }

    public override void Down()
    {
        Delete.Table("billing");
    }
}