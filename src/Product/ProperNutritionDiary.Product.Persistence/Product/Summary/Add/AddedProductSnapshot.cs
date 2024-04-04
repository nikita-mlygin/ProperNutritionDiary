namespace ProperNutritionDiary.Product.Persistence.Product.Summary.Add;

public class AddedProductSnapshot
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public Guid Id { get; set; }
    public DateTime AddedAt { get; set; }
}
