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

    public static ExternalSourceIdentity? Create(ExternalSourceType t, string value)
    {
        return t switch
        {
            ExternalSourceType.Barcode => CreateFromBarcode(value),
            ExternalSourceType.USDA => CreateFromUsdaCode(value),
            _ => null,
        };
    }
}
