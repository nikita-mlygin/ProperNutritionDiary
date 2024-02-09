using DomainDesignLib.Abstractions.Repository;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product;

public interface IProductRepository : IRepository<Product, ProductId>
{
    public List<Product> GetFavoriteProductList(UserId user);
    public Task AddProductToFavoriteList(FavoriteProductLineItem favoriteProductListItem);
    public Task RemoveProductFromFavoriteList(UserId user, ProductId product);
}
