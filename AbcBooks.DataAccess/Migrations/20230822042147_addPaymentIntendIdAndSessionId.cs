using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbcBooks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addPaymentIntendIdAndSessionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentIntendId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntendId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Orders");
        }
    }
}
