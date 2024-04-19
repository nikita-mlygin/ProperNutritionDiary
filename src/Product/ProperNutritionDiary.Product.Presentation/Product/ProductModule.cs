using System.Security.Claims;
using Carter;
using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ProperNutritionDiary.Product.Application.Product.Add;
using ProperNutritionDiary.Product.Application.Product.Get.ById;
using ProperNutritionDiary.Product.Domain.Product.Get;

namespace ProperNutritionDiary.Product.Presentation.Product;

public sealed class ProductModule() : CarterModule("/product")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("test", Test).RequireAuthorization("guest");
        app.MapGet("{id}", GetById).RequireAuthorization("guest");
        app.MapPost("", CreateProduct).RequireAuthorization("plain");
    }

    private static Results<Ok, ForbidHttpResult> Test()
    {
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok<ProductSummary>, BadRequest>> GetById(
        HttpContext ctx,
        [FromQuery] Guid id,
        IMediator mediator
    )
    {
        var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = ctx.User.FindFirstValue(ClaimTypes.Role);

        var query = new GetProductByIdQuery(
            id,
            Guid.Parse(userId!),
            userRole == "admin" ? Domain.User.UserRole.Admin : Domain.User.UserRole.PlainUser
        );

        var res = await mediator.Send(query);

        return TypedResults.Ok(await mediator.Send(query));
    }

    private static async Task<
        Results<Ok<Guid>, BadRequest<string>, ForbidHttpResult>
    > CreateProduct(ClaimsPrincipal u, [FromBody] CreateProductRequest rq, IMediator mediator)
    {
        var id = Guid.Parse(u.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = u.FindFirstValue(ClaimTypes.Role);

        return (
            await mediator.Send(
                new CreateProductCommand(
                    id,
                    role == "admin" ? Domain.User.UserRole.Admin : Domain.User.UserRole.PlainUser,
                    rq.ProductName,
                    rq.Calories,
                    rq.Proteins,
                    rq.Fats,
                    rq.Carbohydrates
                )
            )
        ).Match<Results<Ok<Guid>, BadRequest<string>, ForbidHttpResult>, Guid>(
            res => TypedResults.Ok(res),
            err => TypedResults.BadRequest(err.Message)
        );
    }
}

public class CreateProductRequest
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Calories { get; set; }
    public decimal Proteins { get; set; }
    public decimal Fats { get; set; }
    public decimal Carbohydrates { get; set; }
}

public class ProductSummaryDTO { }
