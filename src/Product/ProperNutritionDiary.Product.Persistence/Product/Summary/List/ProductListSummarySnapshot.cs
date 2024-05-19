using ProperNutritionDiary.UserMenuApi.Product;

namespace ProperNutritionDiary.Product.Persistence.Product.Summary.List;

public class ProductListSummarySnapshot
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? Owner { get; set; }
    public ExternalSourceType? ExternalSourceType { get; set; }
    public string? ExternalSource { get; set; }
}
