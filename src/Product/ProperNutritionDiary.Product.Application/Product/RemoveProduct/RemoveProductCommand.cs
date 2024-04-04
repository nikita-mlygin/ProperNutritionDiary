using DomainDesignLib.Abstractions.Result;
using MediatR;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Application.Product.RemoveProduct;

public sealed record RemoveProductCommand(Guid UserId, UserRole UserRole, Guid ProductId)
    : IRequest<Result>;
