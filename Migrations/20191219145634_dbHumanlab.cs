using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace humanlab.Migrations
{
    public partial class dbHumanlab : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ActivityName = table.Column<string>(maxLength: 40, nullable: false),
                    FixingTime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    CategoryName = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Grids",
                columns: table => new
                {
                    GridId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    ElementsSize = table.Column<int>(nullable: false),
                    GridName = table.Column<string>(maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grids", x => x.GridId);
                });

            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    ElementId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    Audio = table.Column<string>(nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    ElementName = table.Column<string>(maxLength: 40, nullable: false),
                    Image = table.Column<string>(nullable: false),
                    SpeachText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elements", x => x.ElementId);
                    table.ForeignKey(
                        name: "FK_Elements_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityGrids",
                columns: table => new
                {
                    ActivityId = table.Column<int>(nullable: false),
                    GridId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityGrids", x => new { x.ActivityId, x.GridId });
                    table.ForeignKey(
                        name: "FK_ActivityGrids_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityGrids_Grids_GridId",
                        column: x => x.GridId,
                        principalTable: "Grids",
                        principalColumn: "GridId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GridElements",
                columns: table => new
                {
                    GridId = table.Column<int>(nullable: false),
                    ElementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GridElements", x => new { x.GridId, x.ElementId });
                    table.ForeignKey(
                        name: "FK_GridElements_Elements_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Elements",
                        principalColumn: "ElementId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GridElements_Grids_GridId",
                        column: x => x.GridId,
                        principalTable: "Grids",
                        principalColumn: "GridId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityGrids_ActivityId",
                table: "ActivityGrids",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityGrids_GridId",
                table: "ActivityGrids",
                column: "GridId");

            migrationBuilder.CreateIndex(
                name: "IX_Elements_CategoryId",
                table: "Elements",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GridElements_ElementId",
                table: "GridElements",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_GridElements_GridId",
                table: "GridElements",
                column: "GridId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityGrids");

            migrationBuilder.DropTable(
                name: "GridElements");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropTable(
                name: "Grids");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
