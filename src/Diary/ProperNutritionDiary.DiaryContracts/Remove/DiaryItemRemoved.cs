using MassTransit;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryContracts.Remove;

[EntityName("diary-item-removed-event")]
public record DiaryItemRemovedEvent(
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
