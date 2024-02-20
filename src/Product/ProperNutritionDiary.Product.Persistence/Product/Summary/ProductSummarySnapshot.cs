using ProperNutritionDiary.Product.Domain.Macronutrients;

namespace ProperNutritionDiary.Product.Persistence.Product.Summary;

internal class ProductSummarySnapshot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public MacronutrientsSnapshot Macronutrients { get; set; } = null!;
    public Guid? Owner { get; set; }
    public int ViewCount { get; set; }
    public int AddCount { get; set; }
}
