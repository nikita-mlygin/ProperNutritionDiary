namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public class BarcodeProductIdentity : ProductIdentity
{
    public string Barcode { get; set; } = string.Empty;
    public override ProductIdentityType Type
    {
        get => ProductIdentityType.Barcode;
    }
}
