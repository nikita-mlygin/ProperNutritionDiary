namespace ProperNutritionDiary.Product.Application.Product.Update;

using DomainDesignLib.Abstractions.Result;
using MediatR;
using ProperNutritionDiary.Product.Domain.User;

public sealed record UpdateProductCommand(
    Guid UserId,
    UserRole UserRole,
    Guid ProductId,
    string NewName,
    decimal Calories,
    decimal Proteins,
    decimal Fats,
    decimal Carbohydrates
) : IRequest<Result>;
