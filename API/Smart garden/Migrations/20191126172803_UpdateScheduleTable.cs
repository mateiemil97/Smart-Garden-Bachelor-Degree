using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class UpdateScheduleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<float>(
                name: "TemperatureStart",
                table: "Schedule",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoistureReference",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "MoistureStart",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "TemperatureStart",
                table: "Schedule");
        }
    }
}
