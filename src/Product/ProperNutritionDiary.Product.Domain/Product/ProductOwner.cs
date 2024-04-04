namespace ProperNutritionDiary.Product.Domain.Product;

using ProperNutritionDiary.Product.Domain.User;

public record ProductOwner
{
    private ProductOwner(bool isUser, UserId? owner = null)
    {
        this.isUser = isUser;
        Owner = owner;
    }

    private readonly bool isUser;

    public bool IsUser => isUser;

    public bool IsSystem => !isUser;

    public UserId? Owner { get; init; }

    public static ProductOwner ByUser(UserId user) => new(true, user);

    public static ProductOwner BySystem() => new(false);
}
