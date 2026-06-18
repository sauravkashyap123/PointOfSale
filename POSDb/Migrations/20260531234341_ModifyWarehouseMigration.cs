using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSDb.Migrations
{
    /// <inheritdoc />
    public partial class ModifyWarehouseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarehouseCode",
                table: "tblwarehouse",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "tblshopstock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblshopstock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblshopstockHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Shopstockid = table.Column<int>(type: "int", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblshopstockHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblwarehouse_WarehouseCode",
                table: "tblwarehouse",
                column: "WarehouseCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblshopstock");

            migrationBuilder.DropTable(
                name: "tblshopstockHistory");

            migrationBuilder.DropIndex(
                name: "IX_tblwarehouse_WarehouseCode",
                table: "tblwarehouse");

            migrationBuilder.DropColumn(
                name: "WarehouseCode",
                table: "tblwarehouse");
        }
    }
}
