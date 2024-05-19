namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public class BarcodeProductIdentity(string barcode) : ExternalSourceIdentity
{
    public string Barcode { get; set; } = barcode;

    public override ExternalSourceType Type
    {
        get => ExternalSourceType.Barcode;
    }
}
