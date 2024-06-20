using System.Security.Claims;
using Carter;
using LanguageExt.SomeHelp;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProperNutritionDiary.BuildingBlocks.PresentationPackages;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

namespace ProperNutritionDiary.UserStatsApi.UserTotalDailiesStats;

public class UserTotalDailiesStatsModule() : CarterModule("/api/daily")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("", GetByDate);
    }

    public Task<Results<Ok<UserDailyStatsDto>, BadRequest<string>>> GetByDate(
        DateTime date,
        DailyStatsService dailyStatsService,
        ClaimsPrincipal u
    )
    {
        return u.GetUserIdOpt()
            .ToEff(Error.New("User id is invalid"))
            .Bind(ui => dailyStatsService.Find(ui, date))
            .Bind(x => x.ToEff().Map(x => x.Adapt<UserDailyStatsDto>()))
            .Run()
            .Map(f =>
                f.Match<Results<Ok<UserDailyStatsDto>, BadRequest<string>>>(
                    Succ: res => TypedResults.Ok(res),
                    Fail: (e) => TypedResults.BadRequest(e.Message)
                )
            )
            .AsTask();
    }
}

public record UserDailyStatsDto(
    Guid Id,
    Guid UserId,
    DateTime Day,
    Macronutrients TotalMacronutrients,
    decimal TotalWeight
);
