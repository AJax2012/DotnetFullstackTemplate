using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SourceName.Data.Model.Migrations
{
    public partial class CreateLoggingDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogDate = table.Column<DateTime>(nullable: false),
                    Application = table.Column<string>(maxLength: 255, nullable: false),
                    Level = table.Column<string>(maxLength: 50, nullable: false),
                    Message = table.Column<string>(maxLength: 4000, nullable: false),
                    Logger = table.Column<string>(maxLength: 255, nullable: false),
                    CallSite = table.Column<string>(maxLength: 500, nullable: false),
                    Exception = table.Column<string>(maxLength: 2000, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Log");
        }
    }
}
