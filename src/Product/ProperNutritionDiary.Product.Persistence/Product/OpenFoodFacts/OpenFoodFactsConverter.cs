namespace ProperNutritionDiary.Product.Persistence.Product.OpenFoodFacts;

using DomainDesignLib.Abstractions.Result;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;

public static class OpenFoodFactsConverter
{
    public static List<Product>? Convert(OpenApiSearchResponse? res)
    {
        if (res is null)
            return null;

        return res.Products.Select(Convert).ToList();
    }

    public static Product? Convert(OpenFoodFactsProduct? product)
    {
        if (product is null || product.Result.Id == "product_not_found")
            return null;

        MacronutrientsSnapshot macronutrients = GetTypeMacronutrients(product);

        return Product.FromSnapshot(
            new ProductSnapshot()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                FromExternalSource = true,
                Macronutrients = macronutrients,
                Name = product.Product.ProductName,
                ExternalSource = product.Code,
                ExternalSourceType = UserMenuApi.Product.ExternalSourceType.Barcode,
                Owner = null,
                UpdatedAt = null,
            }
        );
    }

    public static Product Convert(OpenApiSearchResponse.OpenApiSearchProduct product)
    {
        MacronutrientsSnapshot macronutrients = GetTypeMacronutrients(product.Nutriments);

        return Product.FromSnapshot(
            new ProductSnapshot()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                FromExternalSource = true,
                Macronutrients = macronutrients,
                Name = product.ProductName,
                ExternalSource = product.Code,
                ExternalSourceType = UserMenuApi.Product.ExternalSourceType.Barcode,
                Owner = null,
                UpdatedAt = null,
            }
        );
    }

    private static MacronutrientsSnapshot GetTypeMacronutrients(OpenFoodFactsProduct product)
    {
        return GetTypeMacronutrients(product.Product.Nutriments);
    }

    private static MacronutrientsSnapshot GetTypeMacronutrients(Nutrients nutrients)
    {
        return new MacronutrientsSnapshot(
            nutrients.EnergyKcal100g,
            nutrients.Proteins100g,
            nutrients.Fat100g,
            nutrients.Carbohydrates100g
        );
    }
}
