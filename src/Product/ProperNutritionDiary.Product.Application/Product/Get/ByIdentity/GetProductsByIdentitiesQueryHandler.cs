using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Application.Product.Get.Search;
using ProperNutritionDiary.Product.Domain.Product.External;
using ProperNutritionDiary.Product.Domain.Product.Identity.Entity;
using ProperNutritionDiary.UserMenuApi.Product;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

namespace ProperNutritionDiary.Product.Application.Product.Get.ByIdentity
{
    public class GetProductsByIdentitiesQueryHandler(
        IExternalProductRepository externalProductRepository,
        ILogger<GetProductsByIdentitiesQueryHandler> logger
    ) : IRequestHandler<GetProductsByIdentitiesQuery, List<ProductSearchItemDto>>
    {
        private readonly IExternalProductRepository repo = externalProductRepository;
        private readonly ILogger<GetProductsByIdentitiesQueryHandler> logger = logger;

        public async Task<List<ProductSearchItemDto>> Handle(
            GetProductsByIdentitiesQuery request,
            CancellationToken cancellationToken
        )
        {
            logger.LogInformation(
                "Handling GetProductsByIdentitiesQuery with {Count} identities",
                request.Ids.Count
            );

            var identities = request
                .Ids.Select(idDto =>
                    idDto.Type == SourceType.System
                        ? null
                        : ExternalSourceIdentity.Create(
                            idDto.Type switch
                            {
                                SourceType.USDA => ExternalSourceType.USDA,
                                SourceType.Barcode => ExternalSourceType.Barcode,
                                SourceType.EdamamRecipe => ExternalSourceType.EdamamRecipe,
                                _
                                    => throw new ArgumentOutOfRangeException(
                                        nameof(idDto.Type),
                                        "Argument out of range"
                                    )
                            },
                            idDto.Value
                        )
                )
                .Where(identity => identity is not null)
                .ToList();

            var products = new List<ExternalProduct>();

            // Fetch products based on their identities
            foreach (var identity in identities)
            {
                try
                {
                    logger.LogInformation("Fetching product for identity {Identity}", identity);

                    if (identity is EdamamRecipeProductIdentity edamamRecipeProductIdentity)
                    {
                        var edamamProducts = await repo.GetByIdentities(
                            new List<EdamamRecipeProductIdentity> { edamamRecipeProductIdentity }
                        );
                        products.AddRange(edamamProducts);
                        logger.LogInformation(
                            "Fetched {Count} products for EdamamRecipeProductIdentity",
                            edamamProducts.Count
                        );
                    }
                    else
                    {
                        var product = await repo.GetByIdentity(identity!);
                        if (product != null)
                        {
                            products.Add(product);
                            logger.LogInformation(
                                "Fetched product {ProductId} for identity {Identity}",
                                product.Id,
                                identity
                            );
                        }
                        else
                        {
                            logger.LogWarning("No product found for identity {Identity}", identity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error fetching product for identity {Identity}", identity);
                }
            }

            var productsDto = products
                .Select(product => new ProductSearchItemDto(
                    product.Id.To(),
                    product.Name,
                    product.Macronutrients,
                    product.Other,
                    null,
                    product.Allergens,
                    product.Ingredients,
                    product.ServingSize
                ))
                .ToList();

            logger.LogInformation("Returning {Count} products", productsDto.Count);
            return productsDto;
        }
    }
}
