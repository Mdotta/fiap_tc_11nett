using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Postech.NETT11.PhaseOne.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameEntitySimplification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Game");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categories",
                table: "Game",
                type: "NVARCHAR(100)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Game",
                type: "DATETIME2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
