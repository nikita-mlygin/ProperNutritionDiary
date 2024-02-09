namespace ProperNutritionDiary.Product.Domain.Product.Create;

using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;

public record ProductCreatedEvent(
    ProductId ProductId,
    string Name,
    Macronutrients Macronutrients,
    ProductOwner? Owner
);
