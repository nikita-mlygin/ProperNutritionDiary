using ProperNutritionDiary.Product.Domain.Product.Identity.Entity;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

namespace ProperNutritionDiary.Product.Domain.Product.External;

public interface IExternalProductRepository
{
    public record Next(int NextUsdaPage, int NextOpenFoodFactsPage, string? NextEdamamRecipePage);

    public Task<ExternalProduct> GetByIdentity(ExternalSourceIdentity id);
    public Task<ExternalProductRepositorySearchResult> Search(string query, Next? next = null);
    public Task<List<ExternalProduct>> GetByIdentities(
        List<EdamamRecipeProductIdentity> identities
    );
}
