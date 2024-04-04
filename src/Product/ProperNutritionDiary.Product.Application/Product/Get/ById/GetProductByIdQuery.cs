using MediatR;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Application.Product.Get.ById;

public sealed record GetProductByIdQuery(Guid ProductId, Guid UserId, UserRole UserRole)
    : IRequest<ProductSummary?>;
