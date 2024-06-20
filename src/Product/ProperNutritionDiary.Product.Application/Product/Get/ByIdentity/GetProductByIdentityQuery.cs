using MediatR;
using ProperNutritionDiary.Product.Application.Product.Get.Search;

namespace ProperNutritionDiary.Product.Application.Product.Get.ByIdentity;

public record GetProductsByIdentitiesQuery(List<ProductIdDto> Ids, Guid? UserId)
    : IRequest<List<ProductSearchItemDto>>;
