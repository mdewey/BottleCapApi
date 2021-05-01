using Microsoft.EntityFrameworkCore.Migrations;

namespace BottleCapApi.Migrations
{
    public partial class AddedSlackname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SlackName",
                table: "DungeonMasters",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlackName",
                table: "DungeonMasters");
        }
    }
}
