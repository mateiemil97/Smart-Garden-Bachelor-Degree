using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class AddStartStopToScheduleEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Schedule",
                newName: "Stop");

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "Schedule",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Start",
                table: "Schedule");

            migrationBuilder.RenameColumn(
                name: "Stop",
                table: "Schedule",
                newName: "DateTime");
        }
    }
}
