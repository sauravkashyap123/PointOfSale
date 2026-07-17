using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSDb.Migrations
{
    /// <inheritdoc />
    public partial class updateproductMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EPurchaseProductModel_IsActive",
                table: "tblProductBase",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EUnit_IsActive",
                table: "tblProductBase",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "tblProductBase",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EPurchaseProductModel_IsActive",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "EUnit_IsActive",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "tblProductBase");
        }
    }
}
