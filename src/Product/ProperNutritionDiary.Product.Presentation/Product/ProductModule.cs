using System.Security.Claims;
using Carter;
using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.BuildingBlocks.PresentationPackages;
using ProperNutritionDiary.Product.Application.Product.Add;
using ProperNutritionDiary.Product.Application.Product.Get;
using ProperNutritionDiary.Product.Application.Product.Get.ById;
using ProperNutritionDiary.Product.Application.Product.Get.Search;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Presentation.Product;

public sealed class ProductModule() : CarterModule("/api/product")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("test", Test).RequireAuthorization("guest");
        app.MapGet("{id}", GetById).RequireAuthorization("canViewProduct");
        app.MapGet("s/{query?}", Search).RequireAuthorization("canViewProduct");
        app.MapPost("", CreateProduct).RequireAuthorization("canCreateProduct");
    }

    private static Results<Ok, ForbidHttpResult> Test()
    {
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok<ProductSummaryDto>, BadRequest>> GetById(
        HttpContext ctx,
        [FromRoute] Guid id,
        ILogger<ProductModule> logger,
        IMediator mediator
    )
    {
        var userId = ctx.User.GetUserId();
        var userRole = ctx.User.FindFirstValue(ClaimTypes.Role);

        var query = new GetProductByIdQuery(
            id,
            userId,
            userRole == "admin" ? Domain.User.UserRole.Admin : Domain.User.UserRole.PlainUser
        );

        var summary = await mediator.Send(query);

        logger.LogInformation("Summary received: {@Summary}", summary);

        return summary.Match<ProductSummary, Results<Ok<ProductSummaryDto>, BadRequest>>(
            summary =>
            {
                var res = summary.Adapt<ProductSummaryDto>();

                res.Id = summary.Id.Value;

                res.Owner = summary.Owner.IsSystem ? null : summary.Owner.Owner!.Value;

                return TypedResults.Ok(res);
            },
            () => TypedResults.BadRequest()
        );
    }

    private static async Task<Results<Ok<List<ProductSearchItemDto>>, BadRequest>> Search(
        [FromRoute] string? query,
        [FromQuery] int? page,
        ClaimsPrincipal u,
        IMediator mediator
    )
    {
        return TypedResults.Ok(
            (await mediator.Send(new ProductSearch(query, u.GetUserId(), UserRole.App, page))).Value
        );
    }

    private static async Task<
        Results<Ok<Guid>, BadRequest<string>, ForbidHttpResult>
    > CreateProduct(ClaimsPrincipal u, [FromBody] CreateProductRequest rq, IMediator mediator)
    {
        var idString = u.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid? id = idString is not null ? Guid.Parse(idString) : null;
        var role = u.FindFirstValue(ClaimTypes.Role);

        return (
            await mediator.Send(
                new CreateProductCommand(
                    id,
                    role switch
                    {
                        "app" => UserRole.App,
                        "admin" => UserRole.Admin,
                        "plain" => UserRole.PlainUser,
                        _ => UserRole.Guest,
                    },
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

public class ProductSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public Macronutrients Macronutrients { get; set; } = default!;
    public Guid? Owner { get; set; }
    public int ViewCount { get; set; }
    public int UseCount { get; set; }
}
