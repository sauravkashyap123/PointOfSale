using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POSDb.Migrations
{
    /// <inheritdoc />
    public partial class StaffMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblStaff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobileno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DOJ = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AadharCardNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Emailid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblStaff", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblStaff");
        }
    }
}
