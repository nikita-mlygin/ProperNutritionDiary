namespace ProperNutritionDiary.Product.Domain.Product;

using ProperNutritionDiary.Product.Domain.Macronutrients;

public class ProductSnapshot
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = null!;
    public double Calories { get; set; }
    public MacronutrientsSnapshot Macronutrients { get; set; } = null!;
    public Guid? Owner { get; set; }
}
