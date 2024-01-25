using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbcBooks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIdColumnToUserCouponTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCoupons_AspNetUsers_ApplicationUserId",
                table: "UserCoupons");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "UserCoupons",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "UserCoupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_OrderId",
                table: "UserCoupons",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoupons_AspNetUsers_ApplicationUserId",
                table: "UserCoupons",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoupons_Orders_OrderId",
                table: "UserCoupons",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCoupons_AspNetUsers_ApplicationUserId",
                table: "UserCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCoupons_Orders_OrderId",
                table: "UserCoupons");

            migrationBuilder.DropIndex(
                name: "IX_UserCoupons_OrderId",
                table: "UserCoupons");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "UserCoupons");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "UserCoupons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoupons_AspNetUsers_ApplicationUserId",
                table: "UserCoupons",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
