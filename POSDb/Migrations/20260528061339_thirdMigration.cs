using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSDb.Migrations
{
    /// <inheritdoc />
    public partial class thirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EPurchaseProductModel_CategoryId",
                table: "tblProductBase",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EPurchaseProductModel_ProductName",
                table: "tblProductBase",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseProductCode",
                table: "tblProductBase",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Warehouseid",
                table: "tblProductBase",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tblpurchaseentry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PurchaseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblpurchaseentry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblpurchasedetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GSTPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GSTAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblpurchasedetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblpurchasedetail_tblpurchaseentry_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "tblpurchaseentry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblpurchasedetail_PurchaseId",
                table: "tblpurchasedetail",
                column: "PurchaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblpurchasedetail");

            migrationBuilder.DropTable(
                name: "tblpurchaseentry");

            migrationBuilder.DropColumn(
                name: "EPurchaseProductModel_CategoryId",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "EPurchaseProductModel_ProductName",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "PurchaseProductCode",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "Warehouseid",
                table: "tblProductBase");
        }
    }
}
