using MassTransit;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryContracts.Add;

[EntityName("diary-item-added-event")]
public record DiaryItemAddedEvent(
    Guid Id,
    DateTime TS,
    SourceType ProductIdType,
    string ProductIdValue,
    decimal Weight,
    Macronutrients Macronutrients,
    Guid UserId,
    DateTime ConsumptionTime,
    ConsumptionType ConsumptionType
);
