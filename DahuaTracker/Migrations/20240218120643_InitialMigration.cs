using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DahuaTracker.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Index = table.Column<string>(type: "TEXT", nullable: true),
                    Count = table.Column<string>(type: "TEXT", nullable: true),
                    PlateNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PlateType = table.Column<string>(type: "TEXT", nullable: true),
                    PlateColor = table.Column<string>(type: "TEXT", nullable: true),
                    VehicleType = table.Column<string>(type: "TEXT", nullable: true),
                    VehicleColor = table.Column<string>(type: "TEXT", nullable: true),
                    VehicleSize = table.Column<string>(type: "TEXT", nullable: true),
                    LaneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    FileName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventInfos");
        }
    }
}
