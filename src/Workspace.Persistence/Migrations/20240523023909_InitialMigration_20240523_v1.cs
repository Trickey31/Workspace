using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workspace.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration_20240523_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Comment",
                type: "timestamp(6) with time zone",
                precision: 6,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) with time zone",
                oldPrecision: 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Comment",
                type: "timestamp(6) with time zone",
                precision: 6,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) with time zone",
                oldPrecision: 6,
                oldNullable: true);
        }
    }
}
