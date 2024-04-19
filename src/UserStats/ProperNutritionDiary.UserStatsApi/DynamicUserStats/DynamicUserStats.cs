namespace ProperNutritionDiary.UserStatsApi.DynamicUserStats;

public class DynamicUserStats
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Weight { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}
