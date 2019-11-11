using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class ChangeNameToIrigationSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_System_SystemId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_System_SystemId",
                table: "Sensor");

            migrationBuilder.DropForeignKey(
                name: "FK_System_User_UserId",
                table: "System");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemState_System_SystemId",
                table: "SystemState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_System",
                table: "System");

            migrationBuilder.RenameTable(
                name: "System",
                newName: "IrigationSystem");

            migrationBuilder.RenameIndex(
                name: "IX_System_UserId",
                table: "IrigationSystem",
                newName: "IX_IrigationSystem_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IrigationSystem",
                table: "IrigationSystem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IrigationSystem_User_UserId",
                table: "IrigationSystem",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_IrigationSystem_SystemId",
                table: "Schedule",
                column: "SystemId",
                principalTable: "IrigationSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_IrigationSystem_SystemId",
                table: "Sensor",
                column: "SystemId",
                principalTable: "IrigationSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemState_IrigationSystem_SystemId",
                table: "SystemState",
                column: "SystemId",
                principalTable: "IrigationSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IrigationSystem_User_UserId",
                table: "IrigationSystem");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_IrigationSystem_SystemId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_IrigationSystem_SystemId",
                table: "Sensor");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemState_IrigationSystem_SystemId",
                table: "SystemState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IrigationSystem",
                table: "IrigationSystem");

            migrationBuilder.RenameTable(
                name: "IrigationSystem",
                newName: "System");

            migrationBuilder.RenameIndex(
                name: "IX_IrigationSystem_UserId",
                table: "System",
                newName: "IX_System_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_System",
                table: "System",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_System_SystemId",
                table: "Schedule",
                column: "SystemId",
                principalTable: "System",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_System_SystemId",
                table: "Sensor",
                column: "SystemId",
                principalTable: "System",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_System_User_UserId",
                table: "System",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemState_System_SystemId",
                table: "SystemState",
                column: "SystemId",
                principalTable: "System",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
