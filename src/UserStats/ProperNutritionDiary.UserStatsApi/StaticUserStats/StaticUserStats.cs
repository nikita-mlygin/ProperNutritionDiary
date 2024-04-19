namespace ProperNutritionDiary.UserStatsApi.StaticUserStats;

public class StaticUserStats
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal? Height { get; set; }
    public string? LifeStyle { get; set; }
}
