using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

namespace ProperNutritionDiary.DiaryApi.Diary;

public class DiaryEntry
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Macronutrients Macronutrients { get; set; } = default!;
    public decimal Weight { get; set; }
    public DateTime ConsumptionTime { get; set; }
}
