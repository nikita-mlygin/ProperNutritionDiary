using DomainDesignLib.Abstractions;

namespace ProperNutritionDiary.Product.Domain.Product.Update;

using ProperNutritionDiary.Product.Domain.Macronutrients;

public class ProductUpdated(
    Guid id,
    string oldName,
    Macronutrients oldMacronutrients,
    Product product
) : DomainEvent(id)
{
    public string OldName { get; init; } = oldName;

    public Macronutrients OldMacronutrients { get; init; } = oldMacronutrients;

    public Product TargetProduct { get; init; } = product;
}
