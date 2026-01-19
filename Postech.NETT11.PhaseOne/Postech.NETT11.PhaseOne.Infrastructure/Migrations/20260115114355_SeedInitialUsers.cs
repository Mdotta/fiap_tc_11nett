using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Postech.NETT11.PhaseOne.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            var clientPasswordHash = BCrypt.Net.BCrypt.HashPassword("Client@123");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "UserHandle", "Username", "PasswordHash", "Role", "IsActive" },
                values: new object[,]
                {
                    {
                        new Guid("6C511C9F-CAE7-4D08-8101-F8AF7C81357A"),
                        new DateTime(2025, 1, 1),
                        "admin",
                        "admin",
                        adminPasswordHash,
                        "Admin",
                        true
                    },
                    {
                        new Guid("4D9C6BD6-821A-40F0-B2B9-64683B5E91E1"),
                        new DateTime(2025, 1, 1),
                        "client",
                        "client",
                        clientPasswordHash,
                        "Client",
                        true
                    }
                });
        }
        
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("6C511C9F-CAE7-4D08-8101-F8AF7C81357A"),
                    new Guid("4D9C6BD6-821A-40F0-B2B9-64683B5E91E1")
                });
        }
    }
}
