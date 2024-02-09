namespace ProperNutritionDiary.Product.Domain.Macronutrients;

public class MacronutrientsSnapshot(
    double calories,
    double proteins,
    double fats,
    double carbohydrates
)
{
    public double Calories { get; set; } = calories;
    public double Proteins { get; set; } = proteins;
    public double Fats { get; set; } = fats;
    public double Carbohydrates { get; set; } = carbohydrates;
}
