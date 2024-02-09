using DomainDesignLib.Abstractions.Repository;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product;

public interface IProductRepository : IRepository<Product, ProductId>
{
    public Task<IEnumerable<Product>> GetFavoriteProductListAsync(UserId user);
    public Task AddProductToFavoriteList(FavoriteProductLineItem favoriteProductListItem);
    public Task RemoveProductFromFavoriteList(UserId user, ProductId product);
}
