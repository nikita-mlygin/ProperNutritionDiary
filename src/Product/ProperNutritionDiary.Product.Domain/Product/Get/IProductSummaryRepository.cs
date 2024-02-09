using DomainDesignLib.Abstractions.Repository;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product.Get;

public interface IProductSummaryRepository
{
    public Task<ProductSummary> GetById(Guid id);
    public Task<ProductSummary> GetAllPopular();
    public Task<ProductSummary> GetAllPopular(UserId user);
    public Task AddView(UserId viewer, ProductId product);
    public Task AddUse(UserId user, ProductId product);
}
