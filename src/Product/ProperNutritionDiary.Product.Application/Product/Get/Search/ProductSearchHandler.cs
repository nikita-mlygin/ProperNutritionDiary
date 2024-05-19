using DomainDesignLib.Abstractions.Result;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;

namespace ProperNutritionDiary.Product.Application.Product.Get.Search;

public class ProductSearchHandler(
    IExternalProductRepository productRepository,
    ILogger<ProductSearchHandler> logger
) : IRequestHandler<ProductSearch, Result<List<ProductListSummary>>>
{
    public async Task<Result<List<ProductListSummary>>> Handle(
        ProductSearch request,
        CancellationToken cancellationToken
    )
    {
        var (products, pageCounts) = await productRepository.Search(request.Query);

        logger.LogInformation("Start adapt product list");

        var res = products.Adapt<List<ProductListSummary>>();

        logger.LogInformation("End adapt product list");

        return res;
    }
}
