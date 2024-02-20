namespace ProperNutritionDiary.Product.Domain.Product.Get;

using ProperNutritionDiary.Product.Domain.User;

public record ProductListSummary(ProductId Id, string Name, ProductOwner Owner) { }
