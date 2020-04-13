using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class AddAutomationModeToSystemState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.AddColumn<bool>(
                name: "AutomationMode",
                table: "SystemState",
                nullable: false,
                defaultValue: false);


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropColumn(
                name: "AutomationMode",
                table: "SystemState");

            migrationBuilder.AddColumn<int>(
                name: "BoardKey",
                table: "IrigationSystem",
                nullable: false,
                defaultValue: 0);

        }
    }
}
