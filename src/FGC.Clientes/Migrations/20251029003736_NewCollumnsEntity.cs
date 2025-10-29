using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGC.Clients.Migrations
{
    /// <inheritdoc />
    public partial class NewCollumnsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Enderecos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Enderecos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Enderecos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Enderecos");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Clientes");
        }
    }
}
