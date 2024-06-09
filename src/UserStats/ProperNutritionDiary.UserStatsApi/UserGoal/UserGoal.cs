using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

namespace ProperNutritionDiary.UserStatsApi.UserGoal;

public class UserGoal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public Macronutrients Goal { get; set; } = null!;
}
