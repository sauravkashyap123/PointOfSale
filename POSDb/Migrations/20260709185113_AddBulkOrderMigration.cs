using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSDb.Migrations
{
    /// <inheritdoc />
    public partial class AddBulkOrderMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdvanceAmount",
                table: "tblProductBase",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "tblProductBase",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "tblProductBase",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "tblProductBase",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EProduction_Remarks",
                table: "tblProductBase",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "tblProductBase",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "tblProductBase",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "tblProductBase",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "tblProductBase",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "tblProductBase",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "tblProductBase",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "tblProductBase",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tblBulkOrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BulkOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBulkOrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblBulkOrderDetail_tblProductBase_BulkOrderId",
                        column: x => x.BulkOrderId,
                        principalTable: "tblProductBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblBulkOrderDetail_BulkOrderId",
                table: "tblBulkOrderDetail",
                column: "BulkOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblBulkOrderDetail");

            migrationBuilder.DropColumn(
                name: "AdvanceAmount",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "EProduction_Remarks",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "tblProductBase");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "tblProductBase");
        }
    }
}
