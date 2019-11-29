using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class UpdateScheduleTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoistureReference",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "MoistureStart",
                table: "Schedule");

            migrationBuilder.AddColumn<bool>(
                name: "Manual",
                table: "Schedule",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manual",
                table: "Schedule");

            migrationBuilder.AddColumn<int>(
                name: "MoistureReference",
                table: "Schedule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MoistureStart",
                table: "Schedule",
                nullable: false,
                defaultValue: 0);
        }
    }
}
