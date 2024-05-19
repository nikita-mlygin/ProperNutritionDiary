namespace ProperNutritionDiary.Product.Domain.Product.Get;

using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

public record ProductListSummary(
    ProductId Id,
    string Name,
    ProductOwner Owner,
    ExternalSourceIdentity? ExternalSource
) { }
