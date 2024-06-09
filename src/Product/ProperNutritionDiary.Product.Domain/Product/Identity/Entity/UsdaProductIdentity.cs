using System.Text.Json.Serialization;

namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public class UsdaProductIdentity(string code) : ExternalSourceIdentity
{
    public string Code { get; private set; } = code;

    public override ExternalSourceType Type
    {
        get => ExternalSourceType.USDA;
    }
}
