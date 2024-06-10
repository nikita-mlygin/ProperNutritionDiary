namespace ProperNutritionDiary.Product.Application.Product.Get.Search;

public record SearchResult(List<ProductSearchItemDto> Products, string? Next);
