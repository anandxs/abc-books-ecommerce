using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbcBooks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryTableInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Discount",
                table: "Categories",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Categories");
        }
    }
}
