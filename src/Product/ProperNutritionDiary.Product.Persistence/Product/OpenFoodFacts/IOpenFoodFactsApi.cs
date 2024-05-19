using System.Text.Json.Serialization;
using Refit;

namespace ProperNutritionDiary.Product.Persistence.Product.OpenFoodFacts;

public interface IOpenFoodFactsApi
{
    [Get("/product/{barcode}?cc={cc}&lc={lc}?fields=nutriments,product_name")]
    public Task<ApiResponse<OpenFoodFactsProduct>> GetByBarcode(
        string barcode,
        string cc = "ru",
        string lc = "ru"
    );
}

public class OpenFoodFactsProduct
{
    public string Code { get; set; } = string.Empty;
    public string[] Errors { get; set; } = [];
    public ProductInfo Product { get; set; } = default!;
    public ResultInfo Result { get; set; } = default!;
    public string Status { get; set; } = string.Empty;
    public string[] Warnings { get; set; } = [];
}

public class ProductInfo
{
    public Nutrients Nutriments { get; set; } = default!;

    [JsonPropertyName("product_name")]
    public string ProductName { get; set; } = string.Empty;
}

public class Nutrients
{
    [JsonPropertyName("carbohydrates")]
    public decimal Carbohydrates { get; set; }

    [JsonPropertyName("carbohydrates_100g")]
    public decimal Carbohydrates100g { get; set; }

    [JsonPropertyName("carbohydrates_serving")]
    public decimal CarbohydratesServing { get; set; }

    [JsonPropertyName("carbohydrates_unit")]
    public string CarbohydratesUnit { get; set; } = string.Empty;

    [JsonPropertyName("carbohydrates_value")]
    public decimal CarbohydratesValue { get; set; }

    [JsonPropertyName("energy")]
    public decimal Energy { get; set; }

    [JsonPropertyName("energy-kcal")]
    public decimal EnergyKcal { get; set; }

    [JsonPropertyName("energy-kcal_100g")]
    public decimal EnergyKcal100g { get; set; }

    [JsonPropertyName("energy-kcal_serving")]
    public decimal EnergyKcalServing { get; set; }

    [JsonPropertyName("energy-kcal_unit")]
    public string EnergyKcalUnit { get; set; } = string.Empty;

    [JsonPropertyName("energy-kcal_value")]
    public decimal EnergyKcalValue { get; set; }

    [JsonPropertyName("energy-kcal_computed")]
    public decimal EnergyKcalValueComputed { get; set; }

    [JsonPropertyName("fat")]
    public decimal Fat { get; set; }

    [JsonPropertyName("fat_100g")]
    public decimal Fat100g { get; set; }

    [JsonPropertyName("fat_serving")]
    public decimal FatServing { get; set; }

    [JsonPropertyName("fat_unit")]
    public string FatUnit { get; set; } = string.Empty;

    [JsonPropertyName("fat_value")]
    public decimal FatValue { get; set; }

    [JsonPropertyName("fiber")]
    public decimal Fiber { get; set; }

    [JsonPropertyName("fiber_100g")]
    public decimal Fiber100g { get; set; }

    [JsonPropertyName("fiber_serving")]
    public decimal FiberServing { get; set; }

    [JsonPropertyName("fiber_unit")]
    public string FiberUnit { get; set; } = string.Empty;

    [JsonPropertyName("fiber_value")]
    public decimal FiberValue { get; set; }

    [JsonPropertyName("proteins")]
    public decimal Proteins { get; set; }

    [JsonPropertyName("proteins_100g")]
    public decimal Proteins100g { get; set; }

    [JsonPropertyName("proteins_serving")]
    public decimal ProteinsServing { get; set; }

    [JsonPropertyName("proteins_unit")]
    public string ProteinsUnit { get; set; } = string.Empty;

    [JsonPropertyName("proteins_value")]
    public decimal ProteinsValue { get; set; }

    [JsonPropertyName("salt")]
    public decimal Salt { get; set; }

    [JsonPropertyName("salt_100g")]
    public decimal Salt100g { get; set; }

    [JsonPropertyName("salt_serving")]
    public decimal SaltServing { get; set; }

    [JsonPropertyName("salt_unit")]
    public string SaltUnit { get; set; } = string.Empty;

    [JsonPropertyName("salt_value")]
    public decimal SaltValue { get; set; }

    [JsonPropertyName("sugar")]
    public decimal Sugars { get; set; }

    [JsonPropertyName("sugar_100g")]
    public decimal Sugars100g { get; set; }

    [JsonPropertyName("sugar_serving")]
    public decimal SugarsServing { get; set; }

    [JsonPropertyName("sugar_unit")]
    public string SugarsUnit { get; set; } = string.Empty;

    [JsonPropertyName("sugar_value")]
    public decimal SugarsValue { get; set; }
}

public class ResultInfo
{
    public string Id { get; set; } = string.Empty;
    public string LcName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
