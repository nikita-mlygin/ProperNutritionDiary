using DomainDesignLib.Abstractions;

namespace ProperNutritionDiary.Product.Domain.Product.Remove;

public class ProductRemoved(Guid id, Product removedProduct) : DomainEvent(id)
{
    public Product RemovedProduct { get; init; } = removedProduct;
}
