using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class AddNameToSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "BoardKey");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "IrigationSystem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "IrigationSystem");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BoardKey",
                nullable: true);
        }
    }
}
