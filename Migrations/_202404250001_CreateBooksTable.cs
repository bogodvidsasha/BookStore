using FluentMigrator;

namespace BookStore.Migrations
{
    [Migration(202404250001)]
    public class CreateBooksTable : Migration
    {
        public override void Up()
        {
            Create.Table("Books")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Title").AsString(100).NotNullable()
                .WithColumn("Author").AsString(100).NotNullable()
                .WithColumn("PublishedYear").AsInt32().Nullable()
                .WithColumn("ISBN").AsString(20).Nullable();
        }

        public override void Down()
        {
            Delete.Table("Books");
        }
    }
}