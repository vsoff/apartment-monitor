using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Apartment.Data.Migrations
{
    public partial class ChangeRegionsColorField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("ColorR", "RegionEntity");
            migrationBuilder.DropColumn("ColorG", "RegionEntity");
            migrationBuilder.DropColumn("ColorB", "RegionEntity");
            migrationBuilder.AddColumn<string>("ColorHex", "RegionEntity", defaultValue: "#FF0000");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>("ColorR", "RegionEntity");
            migrationBuilder.AddColumn<byte>("ColorG", "RegionEntity");
            migrationBuilder.AddColumn<byte>("ColorB", "RegionEntity");
            migrationBuilder.DropColumn("ColorHex", "RegionEntity");
        }
    }
}