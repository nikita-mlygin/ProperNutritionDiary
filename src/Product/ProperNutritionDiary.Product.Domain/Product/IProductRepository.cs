using DomainDesignLib.Abstractions.Repository;

namespace ProperNutritionDiary.Product.Domain.Product;

using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

public interface IProductRepository : IRepository<Product, ProductId>
{
    public Task<IEnumerable<Product>> GetFavoriteProductListAsync(UserId user);
    public Task AddProductToFavoriteListAsync(UserId user, ProductId product, DateTime addedAt);
    public Task RemoveProductFromFavoriteListAsync(UserId user, ProductId product);
    public Task<IEnumerable<UserId>> GetUserWhichFavoriteListContainsProduct(Product product);
    public Task<bool> IsProductInFavoriteList(UserId user, ProductId product);
}
