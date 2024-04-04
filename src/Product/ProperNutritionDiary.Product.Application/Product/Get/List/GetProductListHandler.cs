using MediatR;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;

namespace ProperNutritionDiary.Product.Application.Product.Get.List;

public sealed class GetProductListHandler(IProductSummaryRepository productSummaryRepository)
    : IRequestHandler<GetProductList, List<ProductListSummary>>
{
    private readonly IProductSummaryRepository productSummaryRepository = productSummaryRepository;

    public async Task<List<ProductListSummary>> Handle(
        GetProductList request,
        CancellationToken cancellationToken
    )
    {
        return await productSummaryRepository.GetProductList(
            request.Query,
            request.LastProduct is not null ? new ProductId((Guid)request.LastProduct) : null
        );
    }
}
