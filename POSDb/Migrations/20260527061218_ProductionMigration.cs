using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSDb.Migrations
{
    /// <inheritdoc />
    public partial class ProductionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SaleQuantity",
                table: "tblProductBase",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransferQuantity",
                table: "tblProductBase",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsedQuantity",
                table: "tblProductBase",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleQuantity",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "TransferQuantity",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "UsedQuantity",
                table: "tblProductBase");
        }
    }
}
