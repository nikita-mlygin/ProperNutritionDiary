using DomainDesignLib.Abstractions.Result;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Product.External;

namespace ProperNutritionDiary.Product.Application.Product.Get.Search;

public class ProductSearchHandler(
    IExternalProductRepository productRepository,
    ILogger<ProductSearchHandler> logger
) : IRequestHandler<ProductSearch, Result<SearchResult>>
{
    public async Task<Result<SearchResult>> Handle(
        ProductSearch request,
        CancellationToken cancellationToken
    )
    {
        IExternalProductRepository.Next? nextRequest = null;

        if (
            request.Next is not null
            && request.Next.Split(",", 3)
                is [var nextUsdaPage, var nextOpenFoodFactsPage, var nextRecipePage]
        )
        {
            nextRequest = new IExternalProductRepository.Next(
                int.Parse(nextUsdaPage),
                int.Parse(nextOpenFoodFactsPage),
                nextRecipePage
            );
        }

        var (products, next) = await productRepository.Search(request.Query, nextRequest);

        logger.LogInformation("Start adapt product list");

        var res = products
            .Select(x =>
            {
                return new ProductSearchItemDto(
                    x.Id.To(),
                    x.Name,
                    x.Macronutrients,
                    x.Other,
                    null,
                    x.Allergens,
                    x.Ingredients,
                    ServingSize: x.ServingSize
                );
            })
            .ToList();

        logger.LogInformation("End adapt product list");

        string? nextStr = null;

        if (next is not null)
        {
            nextStr =
                $"{next.NextUsdaPage},{next.NextOpenFoodFactsPage},{next.NextEdamamRecipePage}";
        }

        return new SearchResult(res, nextStr);
    }
}
