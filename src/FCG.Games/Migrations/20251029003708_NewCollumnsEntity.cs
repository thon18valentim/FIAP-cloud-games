using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Games.Migrations
{
    /// <inheritdoc />
    public partial class NewCollumnsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Games",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Games");
        }
    }
}
