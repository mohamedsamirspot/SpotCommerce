using Microsoft.EntityFrameworkCore.Migrations;

namespace SpotCommerce.Data.Migrations
{
    public partial class Lastone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Wishlist");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Wishlist",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
