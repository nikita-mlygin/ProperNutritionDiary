using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Entity;

public class UserMenuItem
{
    public Guid Id { get; set; }
    public Guid ProductIdentityId { get; set; }
    public ProductIdentity ProductId { get; set; } = default!;
    public string ProductName { get; set; } = "";
    public Macronutrients Macronutrients { get; set; } = default!;
    public decimal Weight { get; set; }
    public string? RecipeUrl { get; set; }
    public List<string>? RecipeLines { get; set; }
    public int ConsumptionNumber { get; set; }
}
