using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Smart_garden.Migrations
{
    public partial class AddBoardsKeysTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeriesKey",
                table: "IrigationSystem");

            migrationBuilder.AddColumn<int>(
                name: "BoardKeyId",
                table: "IrigationSystem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BoardKey",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SeriesKey = table.Column<string>(nullable: false),
                    Registered = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardKey", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IrigationSystem_BoardKey_BoardKeyId",
                table: "IrigationSystem");

            migrationBuilder.DropTable(
                name: "BoardKey");

            migrationBuilder.DropIndex(
                name: "IX_IrigationSystem_BoardKeyId",
                table: "IrigationSystem");

            migrationBuilder.DropColumn(
                name: "BoardKeyId",
                table: "IrigationSystem");

            migrationBuilder.AddColumn<string>(
                name: "SeriesKey",
                table: "IrigationSystem",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
