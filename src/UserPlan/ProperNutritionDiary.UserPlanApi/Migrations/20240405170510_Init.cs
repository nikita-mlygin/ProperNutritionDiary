using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProperNutritionDiary.UserPlanApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MacronutrientsGoal_Calories = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MacronutrientsGoal_Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MacronutrientsGoal_Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MacronutrientsGoal_Carbohydrates = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlans", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPlans");
        }
    }
}
