namespace ProperNutritionDiary.Product.Domain.Product;

using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.UserMenuApi.Product;

public class ProductSnapshot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public MacronutrientsSnapshot Macronutrients { get; set; } = null!;
    public bool FromExternalSource { get; set; }
    public Guid? Owner { get; set; }
    public string? ExternalSource { get; set; }
    public ExternalSourceType? ExternalSourceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
