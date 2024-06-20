using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryContracts;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Diary.Get;

public record DiaryEntryResponse(
    Guid Id,
    SourceType IdType,
    string IdValue,
    string ProductName,
    decimal Carbohydrates,
    decimal Proteins,
    decimal Fats,
    decimal Calories,
    decimal Weight,
    DateTime ConsumptionTime,
    ConsumptionType ConsumptionType
);
