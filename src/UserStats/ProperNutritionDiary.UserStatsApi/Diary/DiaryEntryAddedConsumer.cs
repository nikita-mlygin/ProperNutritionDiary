using MassTransit;
using ProperNutritionDiary.DiaryContracts.Add;
using ProperNutritionDiary.UserStatsApi.UserTotalDailiesStats;

namespace ProperNutritionDiary.UserStatsApi.Diary;

public class DiaryItemAddedEventConsumer(
    DailyStatsService dailyStatsService,
    ILogger<DiaryItemAddedEventConsumer> logger
) : IConsumer<DiaryItemAddedEvent>
{
    private readonly DailyStatsService dailyStatsService = dailyStatsService;
    private readonly ILogger<DiaryItemAddedEventConsumer> logger = logger;

    public async Task Consume(ConsumeContext<DiaryItemAddedEvent> context)
    {
        logger.LogInformation("Diary service is: {@DiaryService}", dailyStatsService);

        var res = await dailyStatsService
            .UpdateDailyConsumptionOnAddAsync(
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
