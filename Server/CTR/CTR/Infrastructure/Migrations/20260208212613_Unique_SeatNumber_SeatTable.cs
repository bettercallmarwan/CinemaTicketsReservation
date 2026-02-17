using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Unique_SeatNumber_SeatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Seats_SeatNumber",
                table: "Seats",
                column: "SeatNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seats_SeatNumber",
                table: "Seats");
        }
    }
}
