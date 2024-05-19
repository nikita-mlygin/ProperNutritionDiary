namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public class EdamamProductIdentity(string url) : ProductIdentity
{
    public string Url { get; set; } = url;
    public override ProductIdentityType Type
    {
        get => ProductIdentityType.Edamam;
    }
}
