namespace ProperNutritionDiary.Product.Persistence.Product.Summary.Add;

public class UserStatisticsSnapshot
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int ViewCount { get; set; }
    public int AddCount { get; set; }
}
