using MassTransit;
using ProperNutritionDiary.DiaryContracts.Add;
using ProperNutritionDiary.DiaryContracts.Remove;
using ProperNutritionDiary.UserStatsApi.UserTotalDailiesStats;

namespace ProperNutritionDiary.UserStatsApi.Diary;

public class DiaryItemRemovedEventConsumer(
    DailyStatsService dailyStatsService,
    ILogger<DiaryItemRemovedEvent> logger
) : IConsumer<DiaryItemRemovedEvent>
{
    private readonly DailyStatsService dailyStatsService = dailyStatsService;
    private readonly ILogger<DiaryItemRemovedEvent> logger = logger;

    public async Task Consume(ConsumeContext<DiaryItemRemovedEvent> context)
    {
        logger.LogInformation("Diary service is: {@DiaryService}", dailyStatsService);

        var res = await dailyStatsService
            .UpdateDailyConsumptionOnRemoveAsync(
                context.Message.UserId,
                context.Message.ConsumptionTime,
                context.Message.Macronutrients,
                context.Message.Weight
            )
            .Run();

        if (res.IsSucc)
        {
            logger.LogInformation("Processed successfully: {@Message}", context.Message);
            return;
        }

        res.ThrowIfFail();
    }
}
