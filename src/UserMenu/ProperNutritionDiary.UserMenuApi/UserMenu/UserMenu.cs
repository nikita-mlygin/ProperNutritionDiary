namespace ProperNutritionDiary.UserMenuApi.UserMenu;

public class UserMenu
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public List<UserMenuItem> MenuItems { get; set; } = [];
}
