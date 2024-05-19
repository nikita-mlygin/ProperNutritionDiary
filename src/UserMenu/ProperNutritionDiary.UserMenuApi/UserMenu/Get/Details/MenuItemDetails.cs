using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.UserMenuApi.Product;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Get.Details;

public class MenuItemDetails
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = "";
    public ProductIdentityType ProductIdentityType { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public Macronutrients Macronutrients { get; set; } = default!;
    public decimal Weight { get; set; }
}
