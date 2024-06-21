using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workspace.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _20240416_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ObjKey = table.Column<int>(type: "integer", nullable: false),
                    ObjId = table.Column<Guid>(type: "uuid", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: true, defaultValueSql: "current_timestamp"),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    IsDelete = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
