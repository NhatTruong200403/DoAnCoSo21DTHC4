using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnCNTT.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingPropertiesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "Booking");

            migrationBuilder.RenameColumn(
                name: "IsConfirm",
                table: "Booking",
                newName: "IsRequest");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Booking");

            migrationBuilder.RenameColumn(
                name: "IsRequest",
                table: "Booking",
                newName: "IsConfirm");

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "Booking",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
