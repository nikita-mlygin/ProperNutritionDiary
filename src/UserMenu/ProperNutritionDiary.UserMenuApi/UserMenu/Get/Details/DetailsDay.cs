namespace ProperNutritionDiary.UserMenuApi.UserMenu.Get.Details;

public class DetailsDay
{
    public int DayNumber { get; set; }
    public List<MenuItemDetails> Breakfast { get; set; } = [];
    public List<MenuItemDetails> Lunch { get; set; } = [];
    public List<MenuItemDetails> Dinner { get; set; } = [];
}
