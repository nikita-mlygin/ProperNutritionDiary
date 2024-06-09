namespace ProperNutritionDiary.Product.Domain.Product.External;

using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

public record ExternalProduct(
    ExternalSourceIdentity Id,
    string Name,
    Macronutrients Macronutrients,
    Dictionary<string, decimal> Other,
    decimal ServingSize,
    List<string> Allergens,
    List<string> Ingredients
);
