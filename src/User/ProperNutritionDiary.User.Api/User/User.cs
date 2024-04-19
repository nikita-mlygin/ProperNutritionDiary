namespace ProperNutritionDiary.User.Api.User;

public class User(Guid id, string login, string passwordHash, string role)
{
    public Guid Id { get; set; } = id;
    public string Login { get; set; } = login;
    public string PasswordHash { get; set; } = passwordHash;
    public string Role { get; set; } = role;
}
