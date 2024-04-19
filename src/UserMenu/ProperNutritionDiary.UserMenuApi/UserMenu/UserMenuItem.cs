using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

namespace ProperNutritionDiary.UserMenuApi.UserMenu;

public class UserMenuItem
{
    public Guid Id { get; set; }
    public DateTime ConsumptionTime { get; set; }
    public Guid ProductId { get; set; }
    public Macronutrients Macronutrients { get; set; } = default!;
    public decimal Weight { get; set; }
}
