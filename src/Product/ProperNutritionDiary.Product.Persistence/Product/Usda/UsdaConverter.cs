namespace ProperNutritionDiary.Product.Persistence.Product.Usda;

using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;
using ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

public class UsdaConverter(ILogger<UsdaConverter> logger)
{
    private const string CaloriesMacronutrientNumber = "208";
    private const string ProteinMacronutrientNumber = "203";
    private const string FatMacronutrientNumber = "204";
    private const string CarbohydratesMacronutrientNumber = "205";

    public List<Product>? TryConvert(UsdaSearchResult? results)
    {
        if (results is null)
            return null;

        return results.Foods.Select(TryConvert).Where(x => x is not null).Select(x => x!).ToList();
    }

    public Product? TryConvert(string? productJson)
    {
        if (productJson == null)
            return null;

        var res = JsonConvert.DeserializeObject<BaseUsdaProductItem>(productJson);

        switch (res!.DataType)
        {
            case "Branded":
                var branderRes = JsonConvert.DeserializeObject<BrandedFoodInfo>(productJson);
                return branderRes is null ? null : TryConvert(branderRes);
            case "Foundation":
                var foundationRes = JsonConvert.DeserializeObject<FoundationFoodItem>(productJson);
                return foundationRes is null ? null : TryConvert(foundationRes);
        }

        return null;
    }

    public Product? TryConvert(BrandedFoodInfo usdaProduct)
    {
        Result<MacronutrientsSnapshot> macronutrients = TryGetMacronutrients(usdaProduct);
        Result<string> name = TryGetName(usdaProduct);

        return Result
            .Check(macronutrients.IsFailure, macronutrients.Error)
            .Check(name.IsFailure, name.Error)
            .Success(
                () =>
                    Product.FromSnapshot(
                        new ProductSnapshot()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.Now,
                            FromExternalSource = true,
                            Macronutrients = macronutrients.Value,
                            Name = name.Value,
                            ExternalSource = usdaProduct.FdcId.ToString(),
                            ExternalSourceType = UserMenuApi.Product.ExternalSourceType.USDA,
                            Owner = null,
                            UpdatedAt = null,
                        }
                    )
            )
            .Build()
            .Match<Product?, Product>(r => r, e => null);
    }

    public Product? TryConvert(FoundationFoodItem foundationProduct)
    {
        Result<MacronutrientsSnapshot> macronutrients = TryGetMacronutrients(foundationProduct);
        Result<string> name = TryGetName(foundationProduct);

        return Result
            .Check(macronutrients.IsFailure, macronutrients.Error)
            .Check(name.IsFailure, name.Error)
            .Success(
                () =>
                    Product.FromSnapshot(
                        new ProductSnapshot()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.Now,
                            FromExternalSource = true,
                            Macronutrients = macronutrients.Value,
                            Name = name.Value,
                            ExternalSource = foundationProduct.FdcId.ToString(),
                            ExternalSourceType = UserMenuApi.Product.ExternalSourceType.USDA,
                            Owner = null,
                            UpdatedAt = null,
                        }
                    )
            )
            .Build()
            .Match<Product?, Product>(r => r, e => null);
    }

    public Product? TryConvert(UsdaSearchResult.SearchResultFood product)
    {
        Result<MacronutrientsSnapshot> macronutrients = TryGetMacronutrients(product.FoodNutrients);
        string name = product.Description;

        return Result
            .Check(macronutrients.IsFailure, macronutrients.Error)
            .Success(
                () =>
                    Product.FromSnapshot(
                        new ProductSnapshot()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.Now,
                            FromExternalSource = true,
                            Macronutrients = macronutrients.Value,
                            Name = name,
                            ExternalSource = product.FdcId.ToString(),
                            ExternalSourceType = UserMenuApi.Product.ExternalSourceType.USDA,
                            Owner = null,
                            UpdatedAt = null,
                        }
                    )
            )
            .Build()
            .Match<Product?, Product>(r => r, e => null);
    }

    private Result<string> TryGetName(BrandedFoodInfo usdaProduct)
    {
        return Result.Success(usdaProduct.Description);
    }

    private Result<string> TryGetName(FoundationFoodItem usdaProduct)
    {
        return Result.Success(usdaProduct.Description);
    }

    private static bool IsNull<T>(T? value)
    {
        return value is null;
    }

    private Result<MacronutrientsSnapshot> TryGetMacronutrients(BrandedFoodInfo usdaProduct)
    {
        Result<MacronutrientsSnapshot> macronutrients;

        if (usdaProduct.LabelFoodNutrients is not null)
        {
            macronutrients = Result
                .Check(usdaProduct.LabelFoodNutrients.Calories, IsNull, new Error("Any", "Any"))
                .Check(usdaProduct.LabelFoodNutrients.Protein, IsNull, new Error("Any", "Any"))
                .Check(usdaProduct.LabelFoodNutrients.Fat, IsNull, new Error("Any", "Any"))
                .Check(
                    usdaProduct.LabelFoodNutrients.Carbohydrates,
                    IsNull,
                    new Error("Any", "Any")
                )
                .Success(
                    () =>
                        new MacronutrientsSnapshot(
                            usdaProduct.LabelFoodNutrients.Calories!.Value,
                            usdaProduct.LabelFoodNutrients.Protein!.Value,
                            usdaProduct.LabelFoodNutrients.Fat!.Value,
                            usdaProduct.LabelFoodNutrients.Carbohydrates!.Value
                        )
                );

            return macronutrients;
        }

        if (usdaProduct.FoodNutrients is not null)
        {
            return TryGetMacronutrients(usdaProduct.FoodNutrients);
        }

        return Result.Failure<MacronutrientsSnapshot>(new Error("Any", "Any"));
    }

    private Result<MacronutrientsSnapshot> TryGetMacronutrients(FoundationFoodItem usdaProduct)
    {
        if (usdaProduct.FoodNutrients is not null)
        {
            return TryGetMacronutrients(usdaProduct.FoodNutrients);
        }

        return Result.Failure<MacronutrientsSnapshot>(new Error("Any", "Any"));
    }

    public static Result<MacronutrientsSnapshot> TryGetMacronutrients(FoodNutrient[] foodNutrient)
    {
        decimal? Calories = foodNutrient
            .Where(x => x.Nutrient.Number == CaloriesMacronutrientNumber)
            .Select(x => x.Amount)
            .FirstOrDefault();
        decimal? Protein = foodNutrient
            .Where(x => x.Nutrient.Number == ProteinMacronutrientNumber)
            .Select(x => x.Amount)
            .FirstOrDefault();
        decimal? Fat = foodNutrient
            .Where(x => x.Nutrient.Number == FatMacronutrientNumber)
            .Select(x => x.Amount)
            .FirstOrDefault();
        decimal? Carbohydrates = foodNutrient
            .Where(x => x.Nutrient.Number == CarbohydratesMacronutrientNumber)
            .Select(x => x.Amount)
            .FirstOrDefault();

        return Result
            .Check(Calories, IsNull, new Error("Any", "Any"))
            .Check(Protein, IsNull, new Error("Any", "Any"))
            .Check(Fat, IsNull, new Error("Any", "Any"))
            .Check(Carbohydrates, IsNull, new Error("Any", "Any"))
            .Success(
                () =>
                    new MacronutrientsSnapshot(
                        Calories.Value,
                        Protein.Value,
                        Fat.Value,
                        Carbohydrates.Value
                    )
            );
    }

    public static Result<MacronutrientsSnapshot> TryGetMacronutrients(
        UsdaSearchResult.AbridgedFoodNutrient[] foodNutrient
    )
    {
        decimal? Calories = foodNutrient
            .Where(x =>
                x.Number == CaloriesMacronutrientNumber
                || x.NutrientNumber == CaloriesMacronutrientNumber
            )
            .Select(x => x.Amount != default ? x.Amount : x.Value)
            .FirstOrDefault();
        decimal? Protein = foodNutrient
            .Where(x =>
                x.Number == ProteinMacronutrientNumber
                || x.NutrientNumber == ProteinMacronutrientNumber
            )
            .Select(x => x.Amount != default ? x.Amount : x.Value)
            .FirstOrDefault();
        decimal? Fat = foodNutrient
            .Where(x =>
                x.Number == FatMacronutrientNumber || x.NutrientNumber == FatMacronutrientNumber
            )
            .Select(x => x.Amount != default ? x.Amount : x.Value)
            .FirstOrDefault();
        decimal? Carbohydrates = foodNutrient
            .Where(x =>
                x.Number == CarbohydratesMacronutrientNumber
                || x.NutrientNumber == CarbohydratesMacronutrientNumber
            )
            .Select(x => x.Amount != default ? x.Amount : x.Value)
            .FirstOrDefault();

        return Result
            .Check(Calories, IsNull, new Error("Any", "Any"))
            .Check(Protein, IsNull, new Error("Any", "Any"))
            .Check(Fat, IsNull, new Error("Any", "Any"))
            .Check(Carbohydrates, IsNull, new Error("Any", "Any"))
            .Success(
                () =>
                    new MacronutrientsSnapshot(
                        Calories.Value,
                        Protein.Value,
                        Fat.Value,
                        Carbohydrates.Value
                    )
            );
    }
}
