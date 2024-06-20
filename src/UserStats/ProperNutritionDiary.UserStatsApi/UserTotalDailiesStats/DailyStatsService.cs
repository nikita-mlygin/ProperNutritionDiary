using LanguageExt;
using LanguageExt.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.UserStatsApi.Db;
using ProperNutritionDiary.UserStatsApi.Diary;
using ProperNutritionDiary.UserStatsContracts;

namespace ProperNutritionDiary.UserStatsApi.UserTotalDailiesStats;

public class DailyStatsService(
    IPublishEndpoint publishEndpoint,
    AppCtx ctx,
    ILogger<DailyStatsService> logger
)
{
    private readonly IPublishEndpoint publisher = publishEndpoint;
    private readonly AppCtx ctx = ctx;
    private readonly ILogger<DailyStatsService> logger = logger;

    public Aff<Unit> UpdateDailyConsumptionOnAddAsync(
        Guid userId,
        DateTime date,
        Macronutrients macronutrients,
        decimal weightDelta
    )
    {
        return from dailyConsumption in Find(userId, date)
                .Bind(opt =>
                    opt.Match(
                        Some: x =>
                            LogAndReturn(x)
                                .Bind(x =>
                                {
                                    AddDailyConsumption(x, macronutrients, weightDelta);
                                    return SuccessEff(x);
                                }),
                        None: () =>
                            from st in CreateAndLog(userId, macronutrients, weightDelta, date)
                            from _ in AddToCtx(st)
                            select st
                    )
                )
            from __ in PublishEvent(dailyConsumption)
            from ___ in SaveChangesAsync()
            select unit;
    }

    public Aff<Unit> UpdateDailyConsumptionOnRemoveAsync(
        Guid userId,
        DateTime date,
        Macronutrients macronutrients,
        decimal weightDelta
    )
    {
        return Find(userId, date)
            .Bind(
                (Option<UserTotalDailiesStats> x) =>
                    x.Match(
                        Some: x =>
                            from _ in Eff(
                                () =>
                                    UpdateDailyConsumption(
                                        x,
                                        x.TotalMacronutrients
                                            - (macronutrients * weightDelta / 100),
                                        x.TotalWeight - weightDelta
                                    )
                            )
                            from __ in SaveChangesAsync()
                            select unit,
                        None: () => SuccessEff(unit)
                    )
            );
    }

    private Aff<Unit> AddToCtx(UserTotalDailiesStats stats)
    {
        return ctx.TotalDailiesStats.AddAsync(stats).ToAff().Map(_ => unit);
    }

    private Eff<UserTotalDailiesStats> CreateAndLog(
        Guid userId,
        Macronutrients macronutrients,
        decimal weightDelta,
        DateTime date
    )
    {
        var newEntry = Create(userId, macronutrients * weightDelta / 100, weightDelta, date);

        return Eff(() =>
        {
            logger.LogInformation("Diary entry created: {@DiaryEntry}", newEntry);
            return newEntry;
        });
    }

    private Eff<UserTotalDailiesStats> LogAndReturn(UserTotalDailiesStats entry)
    {
        return Eff(() =>
        {
            logger.LogInformation("Daily consumption received: {@DailyConsumption}", entry);
            return entry;
        });
    }

    async Task<Option<UserTotalDailiesStats>> FetchFromCtx(Guid uId, DateTime date)
    {
        var res = await ctx.TotalDailiesStats.FirstOrDefaultAsync(x =>
            x.UserId == uId && x.Day.Date == date.Date
        );

        logger.LogInformation("Received: {UserTotalDailiesStats}", res);

        if (res is null)
            return None;

        return Some(res);
    }

    public Aff<Option<UserTotalDailiesStats>> Find(Guid userId, DateTime date)
    {
        logger.LogInformation("Find start");

        return FetchFromCtx(userId, date)
            .ToAff()
            .Catch(
                (Error ex) =>
                {
                    logger.LogError(ex, "Exception while getting value from EF Core");
                    return FailAff<Option<UserTotalDailiesStats>>(Error.New(ex));
                }
            );
    }

    public Aff<Option<UserTotalDailiesStats>> GetById(Guid id)
    {
        return Aff(async () =>
            {
                var res = await ctx.TotalDailiesStats.FirstOrDefaultAsync(x => x.Id == id);
                return res is null ? None : Some(res);
            })
            .Do(
                (st) =>
                {
                    logger.LogInformation("Total daily stats received by id: {@DailyStats}", st);
                    return unit;
                }
            );
    }

    public Aff<List<UserTotalDailiesStats>> GetByInterval(Guid userId, DateTime start, DateTime end)
    {
        return ctx
            .TotalDailiesStats.Where(x =>
                x.Day.Date >= start && x.Day.Date <= end && x.UserId == userId
            )
            .OrderBy(x => x.Day)
            .ToListAsync()
            .ToAff()
            .Do(
                (dsl) =>
                {
                    logger.LogInformation(
                        "Total Dailies by interval received: {DailyStatsList}, start: {start}, end: {end}",
                        dsl,
                        start,
                        end
                    );
                    return unit;
                }
            );
    }

    private static UserTotalDailiesStats Create(
        Guid uId,
        Macronutrients macronutrients,
        decimal weightDelta,
        DateTime date
    )
    {
        return new UserTotalDailiesStats
        {
            Id = Guid.NewGuid(),
            UserId = uId,
            Day = date,
            TotalMacronutrients = macronutrients,
            TotalWeight = weightDelta
        };
    }

    private static Unit AddDailyConsumption(
        UserTotalDailiesStats dailyConsumption,
        Macronutrients macronutrients,
        decimal weightDelta
    )
    {
        dailyConsumption.TotalMacronutrients += macronutrients * weightDelta / 100;

        dailyConsumption.TotalWeight += weightDelta;

        return unit;
    }

    private static Unit UpdateDailyConsumption(
        UserTotalDailiesStats dailyConsumption,
        Macronutrients macronutrients,
        decimal weight
    )
    {
        dailyConsumption.TotalMacronutrients = macronutrients;

        dailyConsumption.TotalWeight = weight;

        return unit;
    }

    private Aff<Unit> SaveChangesAsync()
    {
        return ctx.SaveChangesAsync().ToAff().Map(_ => unit);
    }

    private Aff<Unit> PublishEvent(UserTotalDailiesStats dailyConsumption)
    {
        var dailyConsumptionUpdatedEvent = new DailyConsumptionUpdatedEvent(
            dailyConsumption.UserId,
            dailyConsumption.Day,
            dailyConsumption.TotalMacronutrients,
            dailyConsumption.TotalWeight,
            DateTime.UtcNow
        );

        return publisher.Publish(dailyConsumptionUpdatedEvent).ToUnit().ToAff().Map(_ => unit);
    }
}
