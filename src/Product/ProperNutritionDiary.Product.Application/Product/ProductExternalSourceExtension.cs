using ProperNutritionDiary.Product.Application.Product.Get;
using ProperNutritionDiary.Product.Application.Product.Get.ByIdentity;
using ProperNutritionDiary.Product.Domain.Product.Identity.Entity;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

namespace ProperNutritionDiary.Product.Application.Product;

internal static class ProductExternalSourceExtension
{
    internal static ExternalSourceIdentitySummary To(this ExternalSourceIdentity identity)
    {
        ExternalSourceIdentitySummary? res = identity
            .For(
                (UsdaProductIdentity id) =>
                    new ExternalSourceIdentitySummary(SourceType.USDA, id.Code)
            )
            .For(
                (BarcodeProductIdentity id) =>
                    new ExternalSourceIdentitySummary(SourceType.Barcode, id.Barcode)
            )
            .For(
                (EdamamRecipeProductIdentity id) =>
                    new ExternalSourceIdentitySummary(SourceType.EdamamRecipe, id.Uri)
            );

        return res ?? throw new Exception();
    }
}
