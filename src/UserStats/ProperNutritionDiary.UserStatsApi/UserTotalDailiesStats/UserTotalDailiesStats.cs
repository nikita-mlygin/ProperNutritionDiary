using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

namespace ProperNutritionDiary.UserStatsApi.UserTotalDailiesStats;

public class UserTotalDailiesStats
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Day { get; set; }
    public Macronutrients TotalMacronutrients { get; set; } = null!;
}
