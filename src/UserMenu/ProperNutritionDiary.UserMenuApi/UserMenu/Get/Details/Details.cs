using System.Text.Json.Serialization;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Get.Details;

public class Details
{
    public Guid Id { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime Date { get; set; }
    public List<DetailsDay> DailyMenus { get; set; } = [];
}
