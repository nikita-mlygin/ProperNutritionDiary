using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Product.Identity.Entity;

public class SystemProductIdentity(Guid id) : ProductIdentity
{
    public Guid Id { get; set; } = id;

    public override SourceType Type
    {
        get => SourceType.Barcode;
    }
}
