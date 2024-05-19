using ProperNutritionDiary.UserMenuApi.Product;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Create;

public class CreateMenuItem
{
    public ProductItem Product { get; set; } = default!;
    public decimal Weight { get; set; }
}
