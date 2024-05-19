using ProperNutritionDiary.UserMenuApi.Product.Entity;

namespace ProperNutritionDiary.Product.Domain.Product;

public interface IExternalProductRepository
{
    public Task<Product?> GetFromExternalSource(ExternalSourceIdentity externalSourceIdentity);
    public Task<(List<Product>? products, int[] pageCounts)> Search(string query, int page = 1);
}
