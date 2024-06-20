using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryContracts.Product;
using Refit;

namespace ProperNutritionDiary.DiaryApi.Product;

public interface IProductApi
{
    [Post("/product")]
    Task<Guid> CreateProduct(CreateProductRequest rq);

    [Get("/product/{id}")]
    Task<ProductDetails> GetProductById([AliasAs("id")] Guid id);

    [Get("/product/s/{query?}")]
    Task<SearchResult> Search([AliasAs("query")] string? query, [AliasAs("next")] string? next);

    [Post("/product/get")]
    Task<List<ProductSearchItemDto>> GetProductsByIds(GetProductsRequest request);
}

public record SearchResult(List<ProductSearchItemDto> Products, string? Next);

public class ProductDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public Macronutrients Macronutrients { get; set; } = default!;
    public Guid? Owner { get; set; }
    public int ViewCount { get; set; }
    public int UseCount { get; set; }
}

public class CreateProductRequest
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Calories { get; set; }
    public decimal Proteins { get; set; }
    public decimal Fats { get; set; }
    public decimal Carbohydrates { get; set; }
}

public class GetProductsRequest
{
    public List<ProductIdDto> ProductIds { get; set; } = new List<ProductIdDto>();
}

public class ProductSearchItemDto
{
    public ExternalSourceIdentitySummary? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Macronutrients Macronutrients { get; set; } = default!;
    public Dictionary<string, decimal> OtherNutrients { get; set; } = [];
    public ProductOwnerDto? Owner { get; set; }
    public List<string>? Allergens { get; set; }
    public List<string>? Ingredients { get; set; }
    public decimal ServingSize { get; set; }
}

public class ExternalSourceIdentitySummary
{
    public SourceType Type { get; set; }
    public string Value { get; set; } = string.Empty;
}

public record ProductOwnerDto(string OwnerType, string? OwnerIdentity);

public class ProductIdDto
{
    [JsonConverter(typeof(JsonNumberEnumConverter<SourceType>))]
    public SourceType Type { get; set; }
    public string Value { get; set; } = string.Empty;
}
