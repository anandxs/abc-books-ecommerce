using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbcBooks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateTransactinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "transactionType",
                table: "Transactions",
                newName: "TransactionType");

            migrationBuilder.RenameColumn(
                name: "transactionDate",
                table: "Transactions",
                newName: "TransactionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "transactionType");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "Transactions",
                newName: "transactionDate");
        }
    }
}
