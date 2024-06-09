namespace ProperNutritionDiary.Product.Application.Product.Get;

public record ProductListItemDto(
    Guid Id,
    string Name,
    ProductOwnerDto? Owner,
    ExternalSourceIdentitySummary? ExternalSource
);
