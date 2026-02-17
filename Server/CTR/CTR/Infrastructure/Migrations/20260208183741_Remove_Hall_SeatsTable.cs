using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Hall_SeatsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hall",
                table: "Seats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hall",
                table: "Seats",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
