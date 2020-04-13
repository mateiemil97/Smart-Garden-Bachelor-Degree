using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class A : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IrigationSystem_BoardKey_BoardKeyId",
                table: "IrigationSystem");

            migrationBuilder.DropIndex(
                name: "IX_IrigationSystem_BoardKeyId",
                table: "IrigationSystem");

            migrationBuilder.AddColumn<int>(
                name: "BoardKey",
                table: "IrigationSystem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IrigationSystemId",
                table: "BoardKey",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardKey_IrigationSystemId",
                table: "BoardKey",
                column: "IrigationSystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardKey_IrigationSystem_IrigationSystemId",
                table: "BoardKey",
                column: "IrigationSystemId",
                principalTable: "IrigationSystem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardKey_IrigationSystem_IrigationSystemId",
                table: "BoardKey");

            migrationBuilder.DropIndex(
                name: "IX_BoardKey_IrigationSystemId",
                table: "BoardKey");

            migrationBuilder.DropColumn(
                name: "BoardKey",
                table: "IrigationSystem");

            migrationBuilder.DropColumn(
                name: "IrigationSystemId",
                table: "BoardKey");

            migrationBuilder.CreateIndex(
                name: "IX_IrigationSystem_BoardKeyId",
                table: "IrigationSystem",
                column: "BoardKeyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IrigationSystem_BoardKey_BoardKeyId",
                table: "IrigationSystem",
                column: "BoardKeyId",
                principalTable: "BoardKey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
