using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class CreateSensorPortTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Sensor");

            migrationBuilder.AddColumn<int>(
                name: "PortId",
                table: "Sensor",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SensorPort",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Port = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorPort", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_PortId",
                table: "Sensor",
                column: "PortId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_SensorPort_PortId",
                table: "Sensor",
                column: "PortId",
                principalTable: "SensorPort",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_SensorPort_PortId",
                table: "Sensor");

            migrationBuilder.DropTable(
                name: "SensorPort");

            migrationBuilder.DropIndex(
                name: "IX_Sensor_PortId",
                table: "Sensor");

            migrationBuilder.DropColumn(
                name: "PortId",
                table: "Sensor");

            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "Sensor",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
