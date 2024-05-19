namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public class SystemProductIdentity : ProductIdentity
{
    public Guid Guid { get; set; }
    public override ProductIdentityType Type
    {
        get => ProductIdentityType.SystemItem;
    }
}
