using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProperNutritionDiary.UserStatsApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicUserStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicUserStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticUserStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LifeStyle = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticUserStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TotalDailiesStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Calories = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Carbohydrates = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotalDailiesStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaticUserStats_UserId",
                table: "StaticUserStats",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicUserStats");

            migrationBuilder.DropTable(
                name: "StaticUserStats");

            migrationBuilder.DropTable(
                name: "TotalDailiesStats");
        }
    }
}
