using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class AddWaterSwitchToZoneTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WaterSwitch",
                table: "Zone",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterSwitch",
                table: "Zone");
        }
    }
}
