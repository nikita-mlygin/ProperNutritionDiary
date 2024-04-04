using DomainDesignLib.Abstractions;

namespace ProperNutritionDiary.Product.Domain.User;

public class User(UserId id, UserRole role) : Entity<UserId>(id)
{
    public UserRole Role { get; private set; } = role;
}

public enum UserRole
{
    Guest,
    Admin,
    PlainUser
}
