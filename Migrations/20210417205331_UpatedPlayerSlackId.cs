using Microsoft.EntityFrameworkCore.Migrations;

namespace BottleCapApi.Migrations
{
    public partial class UpatedPlayerSlackId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SlackName",
                table: "Players",
                newName: "SlackId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SlackId",
                table: "Players",
                newName: "SlackName");
        }
    }
}
