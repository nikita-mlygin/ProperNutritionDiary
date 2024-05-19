namespace ProperNutritionDiary.Product.Persistence.Product;

using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Persistence.Product.OpenFoodFacts;
using ProperNutritionDiary.Product.Persistence.Product.Usda;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

public class ExternalProductRepository(
    IUsdaApi usdaApi,
    UsdaConverter usdaConverter,
    IOpenFoodFactsApi openFoodFactsApi,
    IOpenFoodFactsSearchApi openFoodFactsSearchApi,
    ILogger<ExternalProductRepository> logger
) : IExternalProductRepository
{
    private readonly IUsdaApi usdaApi = usdaApi;
    private readonly UsdaConverter usdaConverter = usdaConverter;
    private readonly IOpenFoodFactsApi openFoodFactsApi = openFoodFactsApi;
    private readonly IOpenFoodFactsSearchApi openFoodFactsSearchApi = openFoodFactsSearchApi;
    private readonly ILogger<ExternalProductRepository> logger = logger;

    public async Task<(List<Product>? products, int[] pageCounts)> Search(
        string query,
        int page = 1
    )
    {
        var usdaResponse = await usdaApi.Get(query, page);
        var openFoodFactsResponse = await openFoodFactsSearchApi.Search(query);

        logger.LogInformation("UsdaResponse: {@UsdaResponse}", usdaResponse.StatusCode);
        logger.LogInformation(
            "OpenFoodFactsResponse: {@OpenFoodFactsResponse}",
            openFoodFactsResponse.StatusCode
        );

        var res = usdaConverter.TryConvert(usdaResponse.Content);

        res = (res ?? [])
            .Union(OpenFoodFactsConverter.Convert(openFoodFactsResponse.Content) ?? [])
            .ToList();

        return (
            res,
            [usdaResponse.Content?.TotalPages ?? -1, openFoodFactsResponse.Content?.PageCount ?? -1]
        );
    }

    public Task<Product?> GetFromExternalSource(ExternalSourceIdentity externalSourceIdentity)
    {
        Task<Product?>? res = externalSourceIdentity
            .For(
                async (UsdaProductIdentity id) =>
                    usdaConverter.TryConvert((await usdaApi.Get(id.Code)).Content)
            )
            .For(
                async (BarcodeProductIdentity id) =>
                    OpenFoodFactsConverter.Convert(
                        (await openFoodFactsApi.GetByBarcode(id.Barcode)).Content
                    )
            );

        return res ?? Task.FromResult<Product?>(null);
    }
}
