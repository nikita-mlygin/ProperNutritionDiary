using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Entity;

public class UserDailyMenu
{
    public Guid Id { get; set; }
    public int DayNumber { get; set; }
    public List<UserMenuItem> MenuItems { get; set; } = [];

    public Macronutrients GetTotalMacronutrients()
    {
        if (MenuItems.Count == 0)
            return Macronutrients.Create(0, 0, 0, 0).Value;

        return MenuItems
            .Skip(1)
            .Aggregate(
                MenuItems[0].Macronutrients * MenuItems[0].Weight / 100,
                (prev, x) => prev + x.Macronutrients * x.Weight / 100
            );
    }
}
