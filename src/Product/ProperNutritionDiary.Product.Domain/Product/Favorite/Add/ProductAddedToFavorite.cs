using DomainDesignLib.Abstractions;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product.Favorite.Add;

public class ProductAddedToFavorite(Guid id, UserId user, ProductId product) : DomainEvent(id)
{
    public UserId User { get; } = user;
    public ProductId Product { get; } = product;
}
