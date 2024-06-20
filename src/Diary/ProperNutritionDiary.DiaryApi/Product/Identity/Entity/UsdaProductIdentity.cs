using System.Text.Json.Serialization;
using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Product.Identity.Entity;

public class UsdaProductIdentity(string code) : ProductIdentity
{
    public string Code { get; private set; } = code;

    public override SourceType Type
    {
        get => SourceType.USDA;
    }
}
