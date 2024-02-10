namespace ProperNutritionDiary.Product.Persistence.Product.Favorite;

public class FavoriteProductSnapshot(Guid userId, Guid productId, DateTime addedAt)
{
    public Guid UserId { get; set; } = userId;
    public Guid ProductId { get; set; } = productId;
    public DateTime AddedAt { get; set; } = addedAt;
}
