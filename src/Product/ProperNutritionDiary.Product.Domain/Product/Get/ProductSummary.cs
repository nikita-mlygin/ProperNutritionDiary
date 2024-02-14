namespace ProperNutritionDiary.Product.Domain.Product.Get;

using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.User;

public record ProductSummary(
    ProductId Id,
    string Name,
    Macronutrients Macronutrients,
    ProductOwner Owner,
    int ViewCount,
    int UseCount
)
{
    public static ProductSummary FromSnapshot(ProductSummarySnapshot snapshot)
    {
        return new ProductSummary(
            new ProductId(snapshot.Id),
            snapshot.Name,
            Macronutrients.FromSnapshot(snapshot.Macronutrients),
            snapshot.Owner is null
                ? ProductOwner.BySystem()
                : ProductOwner.ByUser(new UserId((Guid)snapshot.Owner)),
            snapshot.ViewCount,
            snapshot.AddCount
        );
    }
}
