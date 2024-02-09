using DomainDesignLib.Abstractions;

namespace ProperNutritionDiary.Product.Domain.Product.Get;

public class ProductReceived(Guid id, ProductId receivedProduct) : DomainEvent(id)
{
    public ProductId ReceivedProduct { get; init; } = receivedProduct;
}
