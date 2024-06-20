using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Carter;
using LanguageExt;
using LanguageExt.Common;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProperNutritionDiary.BuildingBlocks.PresentationPackages;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.DiaryApi.Diary.Create;
using ProperNutritionDiary.DiaryApi.Diary.Get;
using ProperNutritionDiary.DiaryApi.Diary.Update;
using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryApi.Product.Identity.Entity;

namespace ProperNutritionDiary.DiaryApi.Diary;

public class DiaryModule : CarterModule
{
    private const string PlainUserPolicyName = "plain";

    public DiaryModule()
        : base("/api/diary") { }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("", CreateDiary).RequireAuthorization(PlainUserPolicyName);
        app.MapGet("{diaryId}", GetDiary).RequireAuthorization(PlainUserPolicyName);
        app.MapPost("{diaryId}/entry", AddDiaryEntry).RequireAuthorization(PlainUserPolicyName);
        app.MapPost("entry/byDate", AddDiaryEntryFromDay).RequireAuthorization(PlainUserPolicyName);
        app.MapDelete("{entryId}", DeleteDiaryEntry).RequireAuthorization(PlainUserPolicyName);
        app.MapGet("byDate", GetByDate).RequireAuthorization(PlainUserPolicyName);
        app.MapPut("{diaryId}/entry/{entryId}", UpdateDiaryEntry)
            .RequireAuthorization(PlainUserPolicyName);
    }

    private static async Task<Results<Ok<Guid>, BadRequest<string>>> CreateDiary(
        [FromServices] IDiaryService diaryService,
        [FromBody] CreateDiaryRequest request,
        ClaimsPrincipal user
    )
    {
        var userId = user.GetUserId();

        if (userId == null)
            return TypedResults.BadRequest("User id is invalid");

        var result = await diaryService.CreateDiaryAsync(userId.Value, request.Date).Run();

        return result.Match<Results<Ok<Guid>, BadRequest<string>>>(
            id => TypedResults.Ok(id),
            err => TypedResults.BadRequest(err.Message)
        );
    }

    private static async Task<Results<Ok, BadRequest<string>>> UpdateDiary(
        [FromServices] IDiaryService diaryService,
        [FromRoute] Guid diaryId,
        [FromBody] UpdateDiaryRequest request
    )
    {
        var result = await diaryService.UpdateDiaryAsync(diaryId, request.Date).Run();

        return result.Match<Results<Ok, BadRequest<string>>>(
            id => TypedResults.Ok(),
            err => TypedResults.BadRequest(err.Message)
        );
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteDiary(
        [FromServices] IDiaryService diaryService,
        [FromRoute] Guid diaryId
    )
    {
        var result = await diaryService.DeleteDiaryAsync(diaryId).Run();

        return result.Match<Results<Ok, BadRequest<string>>>(
            _ => TypedResults.Ok(),
            err => TypedResults.BadRequest(err.Message)
        );
    }

    private static Task<Results<Ok<DiaryResponse>, BadRequest<string>>> GetDiary(
        [FromServices] IDiaryService diaryService,
        [FromRoute] Guid diaryId
    )
    {
        return diaryService
            .GetDiaryAsync(diaryId)
            .Map(diary => diary.Adapt<DiaryResponse>())
            .Run()
            .Map(result =>
                result.Match<Results<Ok<DiaryResponse>, BadRequest<string>>>(
                    ok => TypedResults.Ok(ok),
                    err => TypedResults.BadRequest(err.Message)
                )
            )
            .AsTask();
    }

    private static Task<Results<Ok, BadRequest<string>>> AddDiaryEntry(
        [FromServices] IDiaryService diaryService,
        [FromRoute] Guid diaryId,
        [FromBody] AddDiaryEntryRequest request
    )
    {
        var productIdentityResult = ProductIdentity.Create(
            request.ProductIdType,
            request.ProductIdValue
        );

        if (productIdentityResult is null)
        {
            return Task.FromResult(
                (Results<Ok, BadRequest<string>>)
                    TypedResults.BadRequest("Invalid product identity or macronutrients")
            );
        }

        return diaryService
            .AddDiaryEntryAsync(
                diaryId,
                productIdentityResult,
                request.Weight,
                request.ConsumptionTime
            )
            .Run()
            .Map(result =>
                result.Match<Results<Ok, BadRequest<string>>>(
                    _ => TypedResults.Ok(),
                    err => TypedResults.BadRequest(err.Message)
                )
            )
            .AsTask();
    }

    private static Task<Results<Ok<Guid>, BadRequest<string>>> AddDiaryEntryFromDay(
        [FromServices] IDiaryService diaryService,
        [FromBody] AddDiaryEntryRequest request,
        ClaimsPrincipal user,
        ILogger<DiaryModule> logger
    )
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Task.FromResult(
                (Results<Ok<Guid>, BadRequest<string>>)TypedResults.BadRequest("User ID not found")
            );
        }

        var productIdentityResult = ProductIdentity.Create(
            request.ProductIdType,
            request.ProductIdValue
        );

        if (productIdentityResult is null)
        {
            return Task.FromResult(
                (Results<Ok<Guid>, BadRequest<string>>)
                    TypedResults.BadRequest("Invalid product identity or macronutrients")
            );
        }

        logger.LogInformation("New product identity: {@Identity}", productIdentityResult);

        return diaryService
            .AddDiaryEntryByDateAsync(
                Guid.Parse(userId),
                request.ConsumptionTime,
                productIdentityResult,
                request.Weight,
                request.ConsumptionTime,
                request.ConsumptionType
            )
            .Run()
            .Map(result =>
                result.Match<Results<Ok<Guid>, BadRequest<string>>>(
                    id => TypedResults.Ok(id),
                    err => TypedResults.BadRequest(err.Message)
                )
            )
            .AsTask();
    }

    private static Task<Results<Ok<DiaryResponse>, BadRequest<string>>> GetByDate(
        [FromQuery] DateTime date,
        [FromServices] IDiaryService diaryService,
        ClaimsPrincipal user
    )
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Task.FromResult(
                (Results<Ok<DiaryResponse>, BadRequest<string>>)
                    TypedResults.BadRequest("User ID not found")
            );
        }

        return diaryService
            .GetDiaryByDateAsync(Guid.Parse(userId), date)
            .Map(diary => new DiaryResponse(
                diary.Id,
                diary.UserId,
                diary.Date,
                diary
                    .DiaryEntries.Select(entry => new DiaryEntryResponse(
                        entry.Id,
                        entry.IdType,
                        entry.IdValue,
                        entry.ProductName,
                        entry.Macronutrients.Carbohydrates,
                        entry.Macronutrients.Proteins,
                        entry.Macronutrients.Fats,
                        entry.Macronutrients.Calories,
                        entry.Weight,
                        entry.ConsumptionTime,
                        entry.ConsumptionType
                    ))
                    .ToList()
            ))
            .Run()
            .Map(result =>
                result.Match<Results<Ok<DiaryResponse>, BadRequest<string>>>(
                    ok => TypedResults.Ok(ok),
                    err => TypedResults.BadRequest(err.Message)
                )
            )
            .AsTask();
    }

    private static async Task<Results<Ok, BadRequest<string>>> DeleteDiaryEntry(
        [FromServices] IDiaryService diaryService,
        [FromRoute] Guid entryId,
        ILogger<DiaryModule> logger
    )
    {
        var result = await diaryService.DeleteDiaryEntryAsync(entryId).Run();

        return result.Match<Results<Ok, BadRequest<string>>>(
            Succ: _ => TypedResults.Ok(),
            Fail: err =>
            {
                if (!err.IsExpected)
                {
                    logger.LogError(err, "Error while delete diary entry async: {@Error}", err);
                    throw err;
                }

                return TypedResults.BadRequest(err.Message);
            }
        );
    }

    private static async Task<Results<Ok, BadRequest<string>>> UpdateDiaryEntry(
        [FromServices] IDiaryService diaryService,
        [FromRoute] Guid diaryId,
        [FromRoute] Guid entryId,
        [FromBody] UpdateDiaryEntryRequest request
    )
    {
        var result = await diaryService
            .UpdateDiaryEntryAsync(diaryId, request.NewWeight, request.ConsumptionType)
            .Run();

        return result.Match<Results<Ok, BadRequest<string>>>(
            _ => TypedResults.Ok(),
            err => TypedResults.BadRequest(err.Message)
        );
    }
}
