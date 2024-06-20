using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryContracts;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Diary.Create;

public record AddDiaryEntryRequest(
    SourceType ProductIdType,
    string ProductIdValue,
    decimal Weight,
    DateTime ConsumptionTime,
    ConsumptionType ConsumptionType
);
