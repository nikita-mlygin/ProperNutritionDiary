using DomainDesignLib.Abstractions.Result;
using MediatR;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Application.Product.Add;

public sealed record CreateProductCommand(
    Guid? UserId,
    UserRole UserRole,
    string ProductName,
    decimal Calories,
    decimal Proteins,
    decimal Fats,
    decimal Carbohydrates
) : IRequest<Result<Guid>>;
