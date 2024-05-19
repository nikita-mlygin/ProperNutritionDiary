namespace ProperNutritionDiary.Product.Domain.Product.Get;

using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

public record ProductSummary(
    ProductId Id,
    string Name,
    Macronutrients Macronutrients,
    ProductOwner Owner,
    ExternalSourceIdentity? ExternalSourceIdentity,
    int ViewCount,
    int UseCount
);
