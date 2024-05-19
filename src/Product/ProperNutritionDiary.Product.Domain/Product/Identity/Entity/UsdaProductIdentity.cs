namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public class UsdaProductIdentity(string code) : ExternalSourceIdentity
{
    public string Code { get; set; } = code;
    public override ExternalSourceType Type
    {
        get => ExternalSourceType.USDA;
    }
}
