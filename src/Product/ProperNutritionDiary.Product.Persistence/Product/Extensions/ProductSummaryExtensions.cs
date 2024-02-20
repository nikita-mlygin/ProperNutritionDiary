using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence.Product.Summary;

namespace ProperNutritionDiary.Product.Persistence.Product.Extensions;

internal static class ProductSummaryExtensions
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
