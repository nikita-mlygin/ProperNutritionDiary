namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public class UsdaProductIdentity : ProductIdentity
{
    public string Code { get; set; } = string.Empty;
    public override ProductIdentityType Type
    {
        get => ProductIdentityType.USDA;
    }
}
