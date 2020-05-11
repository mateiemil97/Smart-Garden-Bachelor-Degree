using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class AAAb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserVegetableId",
                table: "Zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zone_UserVegetableId",
                table: "Zone",
                column: "UserVegetableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Zone_UserVegetableses_UserVegetableId",
                table: "Zone",
                column: "UserVegetableId",
                principalTable: "UserVegetableses",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zone_UserVegetableses_UserVegetableId",
                table: "Zone");

            migrationBuilder.DropIndex(
                name: "IX_Zone_UserVegetableId",
                table: "Zone");

            migrationBuilder.DropColumn(
                name: "UserVegetableId",
                table: "Zone");
        }
    }
}
