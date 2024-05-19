namespace ProperNutritionDiary.UserMenuApi.UserMenu.Create;

public class CreateDaily
{
    public List<CreateMenuItem> Breakfast { get; set; } = [];
    public List<CreateMenuItem> Lunch { get; set; } = [];
    public List<CreateMenuItem> Dinner { get; set; } = [];
}
