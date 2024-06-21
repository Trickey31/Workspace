using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workspace.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration_20240501_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "Notification",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "Notification");
        }
    }
}
