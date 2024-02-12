using DomainDesignLib.Abstractions.Repository;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product;

public interface IProductRepository : IRepository<Product, ProductId>
{
    public Task<IEnumerable<Product>> GetFavoriteProductListAsync(UserId user);
    public Task AddProductToFavoriteList(UserId user, ProductId product, DateTime addedAt);
    public Task RemoveProductFromFavoriteList(UserId user, ProductId product);
    public Task<bool> IsProductInFavoriteList(UserId user, ProductId product);
}
