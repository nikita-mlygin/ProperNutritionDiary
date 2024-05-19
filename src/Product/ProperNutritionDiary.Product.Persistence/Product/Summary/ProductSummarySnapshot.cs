using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.UserMenuApi.Product;

namespace ProperNutritionDiary.Product.Persistence.Product.Summary;

internal class ProductSummarySnapshot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public MacronutrientsSnapshot Macronutrients { get; set; } = null!;
    public ExternalSourceType? ExternalSourceType { get; set; }
    public string? ExternalSource { get; set; }
    public Guid? Owner { get; set; }
    public int ViewCount { get; set; }
    public int AddCount { get; set; }
}
