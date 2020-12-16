using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Apartment.Data.Migrations
{
    public partial class RegionsAddedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegionEntity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ColorR = table.Column<byte>(nullable: false),
                    ColorG = table.Column<byte>(nullable: false),
                    ColorB = table.Column<byte>(nullable: false),
                    PointsJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionEntity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionEntity");
        }
    }
}
