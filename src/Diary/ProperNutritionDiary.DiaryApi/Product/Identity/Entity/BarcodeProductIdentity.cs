using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Product.Identity.Entity;

public class BarcodeProductIdentity : ProductIdentity
{
    public string Barcode { get; set; }

    public override SourceType Type
    {
        get => SourceType.Barcode;
    }

    public BarcodeProductIdentity(string barcode)
    {
        Barcode = barcode;
    }
}
