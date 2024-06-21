using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workspace.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _20240427_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HaveSeen",
                table: "Notification",
                type: "boolean",
                nullable: false,
                defaultValueSql: "false");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notification",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HaveSeen",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notification");
        }
    }
}
