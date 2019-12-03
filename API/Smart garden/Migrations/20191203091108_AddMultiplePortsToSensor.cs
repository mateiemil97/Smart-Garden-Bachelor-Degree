using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class AddMultiplePortsToSensor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sensor_PortId",
                table: "Sensor");

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_PortId",
                table: "Sensor",
                column: "PortId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sensor_PortId",
                table: "Sensor");

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_PortId",
                table: "Sensor",
                column: "PortId",
                unique: true);
        }
    }
}
