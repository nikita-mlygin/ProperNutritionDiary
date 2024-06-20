using MassTransit;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryContracts.Update;

[EntityName("diary-item-updated-event")]
public record DiaryItemUpdatedEvent(
    Guid Id,
    DateTime TS,
    SourceType ProductIdType,
    string ProductIdValue,
    decimal NewWeight,
    decimal OldWeight,
    Macronutrients Macronutrients,
    Guid UserId,
    DateTime ConsumptionTime,
    ConsumptionType ConsumptionType
);
