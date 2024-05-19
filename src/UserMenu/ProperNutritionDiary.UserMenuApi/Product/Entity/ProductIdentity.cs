namespace ProperNutritionDiary.UserMenuApi.Product.Entity;

public abstract class ProductIdentity
{
    public Guid Id { get; set; }
    public abstract ProductIdentityType Type { get; }

    public virtual void For<T>(Action<T> action)
        where T : ProductIdentity
    {
        if (this.GetType().IsAssignableTo(typeof(T)))
        {
            action((T)this);
        }
    }
}
