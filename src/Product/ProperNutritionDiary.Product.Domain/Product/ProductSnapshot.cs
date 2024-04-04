namespace ProperNutritionDiary.Product.Domain.Product;

using ProperNutritionDiary.Product.Domain.Macronutrients;

public class ProductSnapshot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public MacronutrientsSnapshot Macronutrients { get; set; } = null!;
    public Guid? Owner { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
