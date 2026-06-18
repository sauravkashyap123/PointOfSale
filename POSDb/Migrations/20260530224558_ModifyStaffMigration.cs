using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSDb.Migrations
{
    /// <inheritdoc />
    public partial class ModifyStaffMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Shopid",
                table: "tblStaff",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shopid",
                table: "tblStaff");
        }
    }
}
