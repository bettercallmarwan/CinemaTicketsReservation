using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MovieId_ReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hall",
                table: "Seats",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "Reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_MovieId",
                table: "Reservations",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Movies_MovieId",
                table: "Reservations",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Movies_MovieId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_MovieId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Hall",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Reservations");
        }
    }
}
