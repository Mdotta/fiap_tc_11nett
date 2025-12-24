using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "UserHandle", "Username" },
                values: new object[,]
                {
                    { new Guid("4d9c6bd6-821a-40f0-b2b9-64683b5e91e1"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "tempPass", "Client", "client", "client" },
                    { new Guid("6c511c9f-cae7-4d08-8101-f8af7c81357a"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "tempPass", "Admin", "admin", "admin" }
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
