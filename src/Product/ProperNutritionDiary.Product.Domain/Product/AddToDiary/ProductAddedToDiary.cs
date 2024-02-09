using DomainDesignLib.Abstractions;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product.AddToDiary;

public class ProductAddedToDiary(Guid id, ProductId addedProduct, UserId userId) : DomainEvent(id)
{
    public ProductId AddedProduct { get; init; } = addedProduct;
    public UserId UserId { get; init; } = userId;
}
