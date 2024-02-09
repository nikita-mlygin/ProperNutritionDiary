using DomainDesignLib.Abstractions;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product.Favorite.Add;

public class ProductAddedToFavorite(Guid id, FavoriteProductLineItem favoriteProduct)
    : DomainEvent(id)
{
    public FavoriteProductLineItem FavoriteProduct { get; init; } = favoriteProduct;
}
