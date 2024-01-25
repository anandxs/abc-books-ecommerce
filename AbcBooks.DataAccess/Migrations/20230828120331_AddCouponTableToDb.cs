using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbcBooks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountTypeId = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<float>(type: "real", nullable: false),
                    MaxAmount = table.Column<float>(type: "real", nullable: true),
                    ExpirateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsesLeft = table.Column<int>(type: "int", nullable: true),
                    MinimumPurchaseAmount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupons_DiscountTypes_DiscountTypeId",
                        column: x => x.DiscountTypeId,
                        principalTable: "DiscountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_DiscountTypeId",
                table: "Coupons",
                column: "DiscountTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "DiscountTypes");
        }
    }
}
