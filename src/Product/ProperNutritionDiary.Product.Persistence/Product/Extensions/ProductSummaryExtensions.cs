using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.Product.Persistence.Product.Summary;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

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
            snapshot.ExternalSourceType is not null
                ? ExternalSourceIdentity.Create(
                    snapshot.ExternalSourceType.Value,
                    snapshot.ExternalSource!
                )
                : null,
            snapshot.ViewCount,
            snapshot.AddCount
        );
    }
}
