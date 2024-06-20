using MassTransit;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

namespace ProperNutritionDiary.UserStatsContracts;

[EntityName("daily-consumption-updated-event")]
public record DailyConsumptionUpdatedEvent(
    Guid UserId,
    DateTime Date,
    Macronutrients Macronutrients,
    decimal TotalWeight,
    DateTime TS
);
