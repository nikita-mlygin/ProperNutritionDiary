using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryApi.Product.Identity.Entity;
using ProperNutritionDiary.DiaryContracts;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Diary;

public class DiaryEntry
{
    public Guid Id { get; set; }
    public Guid DiaryId { get; set; }
    public Diary Diary { get; set; } = default!;
    public SourceType IdType { get; set; }
    public string IdValue { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public Macronutrients Macronutrients { get; set; } = default!;
    public decimal Weight { get; set; }
    public DateTime ConsumptionTime { get; set; }
    public ConsumptionType ConsumptionType { get; set; }
}
