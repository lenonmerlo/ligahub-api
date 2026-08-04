using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LigaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamSport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sport",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sport",
                table: "Teams");
        }
    }
}
