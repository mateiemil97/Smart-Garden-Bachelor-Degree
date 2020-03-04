using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class AddRangeTempInScheduleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemperatureStart",
                table: "Schedule",
                newName: "TemperatureMin");

            migrationBuilder.AddColumn<float>(
                name: "TemperatureMax",
                table: "Schedule",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemperatureMax",
                table: "Schedule");

            migrationBuilder.RenameColumn(
                name: "TemperatureMin",
                table: "Schedule",
                newName: "TemperatureStart");
        }
    }
}
