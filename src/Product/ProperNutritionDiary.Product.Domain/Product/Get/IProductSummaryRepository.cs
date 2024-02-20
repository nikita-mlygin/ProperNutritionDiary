using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product.Get;

public interface IProductSummaryRepository
{
    public Task<ProductSummary?> GetById(ProductId id);
    public Task<IEnumerable<ProductSummary>> GetAllPopular(int pageNumber);
    public Task<IEnumerable<ProductSummary>> GetAllPopular(UserId user, int pageNumber);
    public Task<List<ProductListSummary>> GetProductList(string nameFilter, ProductId lastProduct);
    public Task AddView(UserId viewer, ProductId product, DateTime viewedAt);
    public Task AddUse(UserId user, ProductId product, DateTime addedAt);
}
