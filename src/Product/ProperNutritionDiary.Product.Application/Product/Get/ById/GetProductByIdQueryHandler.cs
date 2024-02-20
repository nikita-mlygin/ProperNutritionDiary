using DomainDesignLib.Abstractions.Event;
using MediatR;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Application.Product.Get.ById;

public sealed class GetProductByIdQueryHandler(
    IProductSummaryRepository productSummaryRepository,
    IEventDispatcher eventDispatcher
) : IRequestHandler<GetProductByIdQuery, ProductSummary?>
{
    private readonly IProductSummaryRepository productSummaryRepository = productSummaryRepository;
    private readonly IEventDispatcher eventDispatcher = eventDispatcher;

    public async Task<ProductSummary?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        //? может быть вынести логику в доменный сервис

        var id = new ProductId(request.ProductId);
        var res = await productSummaryRepository.GetById(id);
        await eventDispatcher.AddEvent(
            new ProductReceived(Guid.NewGuid(), id, new UserId(request.UserId))
        );

        return res;
    }
}
