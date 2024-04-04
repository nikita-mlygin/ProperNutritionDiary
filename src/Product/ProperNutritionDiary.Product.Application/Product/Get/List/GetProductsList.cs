using MediatR;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Application.Product.Get.List;

public sealed record GetProductList(Guid User, UserRole UserRole, string Query, Guid? LastProduct)
    : IRequest<List<ProductListSummary>>;
