using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AbcBooks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountTypeSeedDAta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DiscountTypes",
                columns: new[] { "Id", "Value" },
                values: new object[,]
                {
                    { 1, "FLAT" },
                    { 2, "PERCENTAGE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DiscountTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DiscountTypes",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
