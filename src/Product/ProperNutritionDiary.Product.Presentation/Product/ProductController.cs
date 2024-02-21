using System.Security.Claims;
using Carter;
using DomainDesignLib.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ProperNutritionDiary.Product.Application.Product.Get.ById;
using ProperNutritionDiary.Product.Domain.Product.Get;

namespace ProperNutritionDiary.Product.Presentation.Product;

public sealed class ProductController(IMediator mediator) : CarterModule("/product")
{
    private readonly IMediator mediator = mediator;

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/product");
    }

    public async Task<Results<Ok<ProductSummary>, BadRequest>> GetById(
        HttpContext ctx,
        [FromQuery] Guid id
    )
    {
        var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = ctx.User.FindFirstValue(ClaimTypes.Role);

        var query = new GetProductByIdQuery(
            id,
            Guid.Parse(userId),
            userRole == "admin" ? Domain.User.UserRole.Admin : Domain.User.UserRole.PlainUser
        );

        var res = await mediator.Send(query);

        return TypedResults.Ok(await mediator.Send(query));
    }
}

public class ProductSummaryDTO { }
