using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace humanlab.Migrations
{
    public partial class Migr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Xposition",
                table: "GridElements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Yposition",
                table: "GridElements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ActivityGrids",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Xposition",
                table: "GridElements");

            migrationBuilder.DropColumn(
                name: "Yposition",
                table: "GridElements");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ActivityGrids");
        }
    }
}
