using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryContracts;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Diary.Create;

public record AddDiaryEntryByDateRequest(
    SourceType ProductIdType,
    string ProductIdValue,
    DateTime ConsumptionTime,
    ConsumptionType ConsumptionType
);
