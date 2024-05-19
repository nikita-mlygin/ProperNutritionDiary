namespace ProperNutritionDiary.UserMenuApi.UserMenu;

using System.Security.Claims;
using Carter;
using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProperNutritionDiary.BuildingBlocks.PresentationPackages;
using ProperNutritionDiary.UserMenuApi.UserMenu.Create;
using ProperNutritionDiary.UserMenuApi.UserMenu.Entity;
using ProperNutritionDiary.UserMenuApi.UserMenu.Get.Details;

public class UserMenuModule() : CarterModule("api")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("", Create).RequireAuthorization();
        app.MapGet("", GetActual).RequireAuthorization();
        app.MapPost("gen", Generate).RequireAuthorization();
    }

    private static async Task<Results<Ok<Guid>, BadRequest<string>, ForbidHttpResult>> Create(
        [FromBody] CreateRequest rq,
        ClaimsPrincipal u,
        UserMenuService ums
    )
    {
        Guid id = Guid.Parse(u.FindFirstValue(ClaimTypes.NameIdentifier)!);
        string role = u.FindFirstValue(ClaimTypes.Role)!;

        return (
            await ums.Create(
                Guid.NewGuid(),
                id,
                role,
                rq.Days.Select(x =>
                {
                    Dictionary<int, CreateMenuItem> r = [];

                    x.Breakfast.ForEach(x => r.Add(1, x));
                    x.Lunch.ForEach(x => r.Add(2, x));
                    x.Dinner.ForEach(x => r.Add(3, x));

                    return r;
                })
                    .ToList()
            )
        ).Match<Results<Ok<Guid>, BadRequest<string>, ForbidHttpResult>, Guid>(
            r => TypedResults.Ok(r),
            err => TypedResults.BadRequest(err.Message)
        );
    }

    private static async Task<Results<Ok<Details>, BadRequest<string>>> GetActual(
        ClaimsPrincipal u,
        UserMenuService ums,
        [FromQuery] DateTime? date = null
    )
    {
        date ??= DateTime.UtcNow;

        var id = u.GetUserId();

        return (await ums.GetActualMenu(id!.Value, date.Value)).Match<
            Results<Ok<Details>, BadRequest<string>>,
            Details
        >(res => TypedResults.Ok(res), err => TypedResults.BadRequest(err.Message));
    }

    private static Task<Results<Ok<Details>, BadRequest<string>>> Generate(
        [FromBody] GenerateMenuConfiguration cfg,
        ClaimsPrincipal u,
        UserMenuService service
    )
    {
        var userId = u.GetUserId();

        return userId.Match<Guid, Task<Results<Ok<Details>, BadRequest<string>>>>(
            async id =>
                (await service.CreateFromEdamam(id, cfg)).Match<
                    Results<Ok<Details>, BadRequest<string>>,
                    UserMenu
                >(
                    r => TypedResults.Ok(UserMenuService.Adapt(r)()),
                    err => TypedResults.BadRequest(err.Message)
                ),
            async () => TypedResults.BadRequest("Ab")
        );
    }
}
