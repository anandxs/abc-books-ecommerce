using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AbcBooks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCashbackOptionsInCouponTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DiscountTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DiscountTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DiscountTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "CashbackMax",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CashbackMin",
                table: "Coupons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CashbackMax",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CashbackMin",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "DiscountTypes",
                columns: new[] { "Id", "Value" },
                values: new object[,]
                {
                    { 1, "FLAT" },
                    { 2, "PERCENTAGE" },
                    { 4, "CASHBACK" }
                });
        }
    }
}
