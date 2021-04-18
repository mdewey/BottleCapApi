using Microsoft.EntityFrameworkCore.Migrations;

namespace BottleCapApi.Migrations
{
    public partial class RenameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnterpriseId",
                table: "Games",
                newName: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "Games",
                newName: "EnterpriseId");
        }
    }
}
