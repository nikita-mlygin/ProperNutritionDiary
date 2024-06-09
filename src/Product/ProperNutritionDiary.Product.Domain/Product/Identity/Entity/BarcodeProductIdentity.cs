namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public class BarcodeProductIdentity : ExternalSourceIdentity
{
    public string Barcode { get; set; }

    public override ExternalSourceType Type
    {
        get => ExternalSourceType.Barcode;
    }

    public BarcodeProductIdentity(string barcode)
    {
        Barcode = barcode;
    }
}
