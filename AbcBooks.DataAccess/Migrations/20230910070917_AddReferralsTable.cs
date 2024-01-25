using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbcBooks.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Referrals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferrerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferredId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFulfilled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referrals", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Referrals");
        }
    }
}
