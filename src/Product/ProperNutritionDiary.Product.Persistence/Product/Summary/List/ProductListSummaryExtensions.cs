using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Domain.Product.Get;
using ProperNutritionDiary.Product.Domain.User;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

namespace ProperNutritionDiary.Product.Persistence.Product.Summary.List;

public static class ProductListSummaryExtensions
{
    public static ProductListSummary To(this ProductListSummarySnapshot snapshot)
    {
        return new ProductListSummary(
            new ProductId(snapshot.Id),
            snapshot.Name,
            snapshot.Owner is null
                ? ProductOwner.BySystem()
                : ProductOwner.ByUser(new UserId(snapshot.Id)),
            snapshot.ExternalSourceType is null || snapshot.ExternalSource is null
                ? null
                : ExternalSourceIdentity.Create(
                    snapshot.ExternalSourceType.Value,
                    snapshot.ExternalSource
                )
        );
    }
}
