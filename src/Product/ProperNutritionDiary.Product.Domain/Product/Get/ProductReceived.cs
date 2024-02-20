using DomainDesignLib.Abstractions;
using ProperNutritionDiary.Product.Domain.User;

namespace ProperNutritionDiary.Product.Domain.Product.Get;

public class ProductReceived(Guid id, ProductId receivedProduct, UserId userId) : DomainEvent(id)
{
    public ProductId ReceivedProduct { get; init; } = receivedProduct;
    public UserId User { get; set; } = userId;
}
