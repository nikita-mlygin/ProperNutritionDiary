using ProperNutritionDiary.Product.Domain.Product.Identity.Entity;

namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public abstract class ExternalSourceIdentity
{
    public abstract ExternalSourceType Type { get; }

    public static BarcodeProductIdentity CreateFromBarcode(string barcode)
    {
        return new BarcodeProductIdentity(barcode);
    }

    public static UsdaProductIdentity CreateFromUsdaCode(string code)
    {
        return new UsdaProductIdentity(code);
    }

    public static EdamamRecipeProductIdentity CreateFromEdamamRecipe(string uri)
    {
        return new EdamamRecipeProductIdentity(uri);
    }

    public static ExternalSourceIdentity? Create(ExternalSourceType t, string value)
    {
        return t switch
        {
            ExternalSourceType.Barcode => CreateFromBarcode(value),
            ExternalSourceType.USDA => CreateFromUsdaCode(value),
            ExternalSourceType.EdamamRecipe => CreateFromEdamamRecipe(value),
            _ => null,
        };
    }
}
