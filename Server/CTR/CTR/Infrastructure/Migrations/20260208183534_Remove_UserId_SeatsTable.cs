using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remove_UserId_SeatsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_User_UserId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_UserId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Seats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Seats",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_UserId",
                table: "Seats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_User_UserId",
                table: "Seats",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
