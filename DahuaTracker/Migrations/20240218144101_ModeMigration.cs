using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DahuaTracker.Migrations
{
    public partial class ModeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "EventInfos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "EventInfos");
        }
    }
}
