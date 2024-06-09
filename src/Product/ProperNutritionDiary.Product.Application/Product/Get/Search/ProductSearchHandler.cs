using DomainDesignLib.Abstractions.Result;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.Product.Domain.Product.External;

namespace ProperNutritionDiary.Product.Application.Product.Get.Search;

public class ProductSearchHandler(
    IExternalProductRepository productRepository,
    ILogger<ProductSearchHandler> logger
) : IRequestHandler<ProductSearch, Result<List<ProductSearchItemDto>>>
{
    public async Task<Result<List<ProductSearchItemDto>>> Handle(
        ProductSearch request,
        CancellationToken cancellationToken
    )
    {
        IExternalProductRepository.Next? nextRequest = null;

        if (request.Next is not null)
        {
            [var a1, var a2, var a3] = request.Next.Split(",", 3);
        }

        var (products, next) = await productRepository.Search(request.Query, nextRequest);

        logger.LogInformation("Start adapt product list");

        var res = products
            .Select(x =>
            {
                return new ProductSearchItemDto(x.Id.To(), x.Name, x.Macronutrients, null);
            })
            .ToList();

        logger.LogInformation("End adapt product list");

        return res;
    }
}
