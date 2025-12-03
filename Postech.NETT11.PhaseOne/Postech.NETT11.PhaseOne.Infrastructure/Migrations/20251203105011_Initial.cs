using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Postech.NETT11.PhaseOne.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    UserHandle = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Username = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR(10)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
