namespace ProperNutritionDiary.Product.Persistence.ExternalProduct;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product.External;
using ProperNutritionDiary.Product.Domain.Product.Identity.Entity;
using ProperNutritionDiary.UserMenuApi.Product;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

public class ExternalProductRepository(
    IExternalProductApi api,
    ILogger<ExternalProductRepository> logger
) : IExternalProductRepository
{
    private readonly IExternalProductApi api = api;
    private readonly ILogger<ExternalProductRepository> logger = logger;

    public async Task<List<ExternalProduct>> GetByIdentities(
        List<EdamamRecipeProductIdentity> identities
    )
    {
        try
        {
            var uris = identities.Select(id => id.Uri).ToList();
            var products = await api.GetFoodFromEdamamAsync(uris);
            return products.Select(ToExternalProduct).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching products by identities");
            throw;
        }
    }

    public Task<ExternalProduct> GetByIdentity(ExternalSourceIdentity id)
    {
        try
        {
            Task<ExternalProduct>? product = id.For(
                    async (UsdaProductIdentity id) =>
                        ToExternalProduct(
                            await api.GetFoodAsync(id.Code, "usda"),
                            ExternalSourceType.USDA
                        )
                )
                .For(
                    async (BarcodeProductIdentity id) =>
                        ToExternalProduct(
                            await api.GetFoodAsync(id.Barcode, "openfoodfacts"),
                            ExternalSourceType.Barcode
                        )
                )
                .For(
                    async (EdamamRecipeProductIdentity id) =>
                        ToExternalProduct((await api.GetFoodFromEdamamAsync([id.Uri]))[0])
                );

            return product ?? throw new Exception();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching product by identity");
            throw;
        }
    }

    public async Task<ExternalProductRepositorySearchResult> Search(
        string query,
        IExternalProductRepository.Next? next = null
    )
    {
        var nextUsdaPage = next.NextUsdaPage;
        var nextOpenFoodFacts = next.NextOpenFoodFactsPage;
        var nextEdamamRecipePage = next.NextEdamamRecipePage;

        try
        {
            List<ExternalProduct> results = [];
            int? lastUsdaPage = null;
            int? lastOpenFoodFactsPage = null;
            string? lastEdamamRecipePage = null;

            if (nextUsdaPage != -1)
            {
                var usdaResult = await api.SearchFoodAsync(
                    query,
                    page: nextUsdaPage,
                    source: "usda"
                );

                results =
                [
                    ..results,
                    ..usdaResult.Item1.Select(x => ToExternalProduct(x, ExternalSourceType.USDA))
                ];

                lastUsdaPage = usdaResult.Item2[0];
            }

            if (nextOpenFoodFacts != -1)
            {
                var openFoodFactsResult = await api.SearchFoodAsync(
                    query,
                    page: nextOpenFoodFacts,
                    source: "openfoodfacts"
                );

                results =
                [
                    ..results,
                    ..openFoodFactsResult.Item1.Select(x => ToExternalProduct(x, ExternalSourceType.Barcode))
                ];

                lastOpenFoodFactsPage = openFoodFactsResult.Item2[0];
            }

            if (nextEdamamRecipePage is not null)
            {
                var edamamRecipeResult = await api.SearchEdamamRecipesAsync(
                    query,
                    cont: nextEdamamRecipePage
                );

                results = [..results, ..edamamRecipeResult.Item1.Select( ToExternalProduct)];
                lastEdamamRecipePage = edamamRecipeResult.Item2;
            }

            return new ExternalProductRepositorySearchResult(
                results,
                new IExternalProductRepository.Next(
                    lastUsdaPage is not null && nextUsdaPage != -1 && nextUsdaPage < lastUsdaPage
                        ? nextUsdaPage + 1
                        : -1,
                    lastOpenFoodFactsPage is not null
                    && nextOpenFoodFacts != -1
                    && nextOpenFoodFacts < lastOpenFoodFactsPage
                        ? nextOpenFoodFacts + 1
                        : -1,
                    lastEdamamRecipePage
                )
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching products");
            throw;
        }
    }

    private ExternalProduct ToExternalProduct(
        IExternalProductApi.StandardFood food,
        ExternalSourceType type
    )
    {
        return new ExternalProduct(
            ExternalSourceIdentity.Create(type, food.Id) ?? throw new Exception(),
            food.Name,
            Macronutrients
                .Create(
                    food.Nutrients.GetValueOrDefault("calories", 0),
                    food.Nutrients.GetValueOrDefault("protein", 0),
                    food.Nutrients.GetValueOrDefault("fat", 0),
                    food.Nutrients.GetValueOrDefault("carbohydrates", 0)
                )
                .Value,
            food.Nutrients,
            (decimal)food.ServingSize,
            food.Allergens,
            food.Ingredients
        );
    }

    private ExternalProduct ToExternalProduct(IExternalProductApi.FoodWithRecipe food)
    {
        return new ExternalProduct(
            ExternalSourceIdentity.Create(ExternalSourceType.EdamamRecipe, food.Id)
                ?? throw new Exception(),
            food.Name,
            Macronutrients
                .Create(
                    food.Nutrients.GetValueOrDefault("calories", 0),
                    food.Nutrients.GetValueOrDefault("protein", 0),
                    food.Nutrients.GetValueOrDefault("fat", 0),
                    food.Nutrients.GetValueOrDefault("carbohydrates", 0)
                )
                .Value,
            food.Nutrients,
            (decimal)food.ServingSize,
            food.Allergens,
            food.Ingredients
        );
    }
}
