using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnCNTT.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPayBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPay",
                table: "Booking",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPay",
                table: "Booking");
        }
    }
}
