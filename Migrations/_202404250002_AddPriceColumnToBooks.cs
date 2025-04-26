using FluentMigrator;

namespace BookStore.Migrations
{
    [Migration(202404250002)]
    public class AddPriceColumnToBooks : Migration
    {
        public override void Up()
        {
            Alter.Table("Books")
                .AddColumn("Price").AsDecimal(10, 2).Nullable();
        }

        public override void Down()
        {
            Delete.Column("Price").FromTable("Books");
        }
    }
}