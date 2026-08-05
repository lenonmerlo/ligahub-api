using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LigaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_TeamId_Name",
                table: "Players");

            migrationBuilder.Sql(
                "DELETE FROM [Players];");

            migrationBuilder.AddColumn<DateOnly>(
                name: "BirthDate",
                table: "Players",
                type: "date",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "JerseyNumber",
                table: "Players",
                type: "int",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Sex",
                table: "Players",
                type: "int",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId_JerseyNumber",
                table: "Players",
                columns: new[] { "TeamId", "JerseyNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_TeamId_JerseyNumber",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "JerseyNumber",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Sex",
                table: "Players");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId_Name",
                table: "Players",
                columns: new[] { "TeamId", "Name" },
                unique: true);
        }
    }
}