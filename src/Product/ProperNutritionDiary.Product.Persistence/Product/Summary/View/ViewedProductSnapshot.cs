namespace ProperNutritionDiary.Product.Persistence.Product.Summary.View;

public class ViewedProductSnapshot
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Guid Id { get; set; }
    public DateTime ViewedAt { get; set; }
}
