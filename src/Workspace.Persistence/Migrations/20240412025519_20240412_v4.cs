using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workspace.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _20240412_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users_Projects",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) with time zone",
                oldPrecision: 6,
                oldNullable: true,
                oldDefaultValueSql: "current_timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Task",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) with time zone",
                oldPrecision: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Task",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) with time zone",
                oldPrecision: 6);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Task",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) with time zone",
                oldPrecision: 6,
                oldNullable: true,
                oldDefaultValueSql: "current_timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Project",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) with time zone",
                oldPrecision: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Project",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(6) with time zone",
                oldPrecision: 6,
                oldNullable: true,
                oldDefaultValueSql: "current_timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Users_Projects",
                type: "timestamp(6) with time zone",
                precision: 6,
                nullable: true,
                defaultValueSql: "current_timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Task",
                type: "timestamp(6) with time zone",
                precision: 6,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Task",
                type: "timestamp(6) with time zone",
                precision: 6,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Task",
                type: "timestamp(6) with time zone",
                precision: 6,
                nullable: true,
                defaultValueSql: "current_timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Project",
                type: "timestamp(6) with time zone",
                precision: 6,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Project",
                type: "timestamp(6) with time zone",
                precision: 6,
                nullable: true,
                defaultValueSql: "current_timestamp",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }
    }
}
