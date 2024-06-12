using ProperNutritionDiary.Product.Domain.Macronutrients;

namespace ProperNutritionDiary.Product.Application.Product.Get.Search;

public record ProductSearchItemDto(
    ExternalSourceIdentitySummary? Id,
    string Name,
    Macronutrients Macronutrients,
    Dictionary<string, decimal> OtherNutrients,
    ProductOwnerDto? Owner,
    List<string>? Allergens,
    List<string>? Ingredients,
    decimal ServingSize
);
