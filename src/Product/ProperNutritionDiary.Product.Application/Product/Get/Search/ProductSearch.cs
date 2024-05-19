namespace ProperNutritionDiary.Product.Application.Product.Get.Search;

using DomainDesignLib.Abstractions.Result;
using MediatR;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;

public record ProductSearch(string Query, Guid? UserId, UserRole UserRole, int? Page = default)
    : IRequest<Result<List<ProductListSummary>>>;