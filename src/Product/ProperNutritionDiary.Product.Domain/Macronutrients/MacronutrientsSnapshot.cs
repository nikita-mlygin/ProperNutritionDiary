namespace ProperNutritionDiary.Product.Domain.Macronutrients;

public class MacronutrientsSnapshot(
    decimal calories,
    decimal proteins,
    decimal fats,
    decimal carbohydrates
)
{
    public decimal Calories { get; set; } = calories;
    public decimal Proteins { get; set; } = proteins;
    public decimal Fats { get; set; } = fats;
    public decimal Carbohydrates { get; set; } = carbohydrates;
}
