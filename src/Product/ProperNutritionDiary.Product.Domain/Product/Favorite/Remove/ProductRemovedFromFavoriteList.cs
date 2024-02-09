using DomainDesignLib.Abstractions;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product.Favorite.Remove;

public class ProductRemovedFromFavoriteList(Guid id, UserId user, ProductId product)
    : DomainEvent(id)
{
    public UserId User { get; init; } = user;
    public ProductId Product { get; init; } = product;
}
