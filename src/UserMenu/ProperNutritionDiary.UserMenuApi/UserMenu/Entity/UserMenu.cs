using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Entity;

public class UserMenu
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public List<UserDailyMenu> DailyMenus { get; set; } = [];
}
