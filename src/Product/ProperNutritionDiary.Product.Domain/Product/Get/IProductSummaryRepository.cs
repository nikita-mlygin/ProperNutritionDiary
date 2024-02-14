using DomainDesignLib.Abstractions.Repository;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product.Get;

public interface IProductSummaryRepository
{
    public Task<ProductSummary?> GetById(ProductId id);
    public Task<ProductSummary> GetAllPopular();
    public Task<ProductSummary> GetAllPopular(UserId user);
    public Task AddView(UserId viewer, ProductId product, DateTime viewedAt);
    public Task AddUse(UserId user, ProductId product, DateTime addedAt);
}
