using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Product.Identity.Entity;

public abstract class ProductIdentity
{
    public abstract SourceType Type { get; }

    public static BarcodeProductIdentity CreateFromBarcode(string barcode)
    {
        return new BarcodeProductIdentity(barcode);
    }

    public static UsdaProductIdentity CreateFromUsdaCode(string code)
    {
        return new UsdaProductIdentity(code);
    }

    public static SystemProductIdentity CreateFromId(Guid id)
    {
        return new SystemProductIdentity(id);
    }

    public static EdamamRecipeProductIdentity CreateFromEdamamRecipe(string uri)
    {
        return new EdamamRecipeProductIdentity(uri);
    }

    public static ProductIdentity? Create(SourceType t, string value)
    {
        Console.WriteLine(string.Format("PRODUCT SRC TYPE: {0}", t));

        return t switch
        {
            SourceType.USDA => CreateFromUsdaCode(value),
            SourceType.Barcode => CreateFromBarcode(value),
            SourceType.EdamamRecipe => CreateFromEdamamRecipe(value),
            SourceType.System => CreateFromId(Guid.Parse(value)),
            _ => null,
        };
    }
}
