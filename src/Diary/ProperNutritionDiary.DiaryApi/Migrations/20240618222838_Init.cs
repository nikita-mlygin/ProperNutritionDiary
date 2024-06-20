using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProperNutritionDiary.DiaryApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiaryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdType = table.Column<int>(type: "int", nullable: false),
                    IdValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Macronutrients_Calories = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Macronutrients_Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Macronutrients_Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Macronutrients_Carbohydrates = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConsumptionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumptionType = table.Column<int>(type: "int", nullable: false, defaultValue: 3)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiaryEntries_Diaries_DiaryId",
                        column: x => x.DiaryId,
                        principalTable: "Diaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_DiaryId",
                table: "DiaryEntries",
                column: "DiaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiaryEntries");

            migrationBuilder.DropTable(
                name: "Diaries");
        }
    }
}
