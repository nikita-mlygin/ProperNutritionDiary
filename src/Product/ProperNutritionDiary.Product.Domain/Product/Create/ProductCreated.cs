using DomainDesignLib.Abstractions;

namespace ProperNutritionDiary.Product.Domain.Product.Create;

public class ProductCreated(Guid id, Product createdProduct) : DomainEvent(id)
{
    public Product CreatedProduct { get; init; } = createdProduct;
}
