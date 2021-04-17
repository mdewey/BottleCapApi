using Microsoft.EntityFrameworkCore.Migrations;

namespace BottleCapApi.Migrations
{
    public partial class AddedEnterpriseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnterpriseId",
                table: "Games",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnterpriseId",
                table: "Games");
        }
    }
}
