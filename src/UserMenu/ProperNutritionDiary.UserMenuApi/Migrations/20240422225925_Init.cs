using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProperNutritionDiary.UserMenuApi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    identityType = table.Column<int>(type: "int", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductIdentities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyMenus_UserMenus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "UserMenus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserMenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductIdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Macronutrients_Calories = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Macronutrients_Proteins = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Macronutrients_Fats = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Macronutrients_Carbohydrates = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConsumptionNumber = table.Column<int>(type: "int", nullable: false),
                    UserMenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMenuItems_DailyMenus_UserMenuId",
                        column: x => x.UserMenuId,
                        principalTable: "DailyMenus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserMenuItems_ProductIdentities_ProductIdentityId",
                        column: x => x.ProductIdentityId,
                        principalTable: "ProductIdentities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenus_MenuId",
                table: "DailyMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMenuItems_ProductIdentityId",
                table: "UserMenuItems",
                column: "ProductIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMenuItems_UserMenuId",
                table: "UserMenuItems",
                column: "UserMenuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMenuItems");

            migrationBuilder.DropTable(
                name: "DailyMenus");

            migrationBuilder.DropTable(
                name: "ProductIdentities");

            migrationBuilder.DropTable(
                name: "UserMenus");
        }
    }
}
