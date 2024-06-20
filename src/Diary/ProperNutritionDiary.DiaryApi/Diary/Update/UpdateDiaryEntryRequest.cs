using ProperNutritionDiary.DiaryContracts;

namespace ProperNutritionDiary.DiaryApi.Diary.Update;

public class UpdateDiaryEntryRequest
{
    public decimal NewWeight { get; set; }
    public ConsumptionType ConsumptionType { get; set; }
}
