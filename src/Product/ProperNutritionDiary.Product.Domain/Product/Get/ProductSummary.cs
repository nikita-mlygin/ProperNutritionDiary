namespace ProperNutritionDiary.Product.Domain.Product.Get;

using ProperNutritionDiary.Product.Domain.Macronutrients;

public record ProductSummary(
    ProductId Id,
    string Name,
    Macronutrients Macronutrients,
    ProductOwner Owner,
    int ViewCount,
    int UseCount
);
