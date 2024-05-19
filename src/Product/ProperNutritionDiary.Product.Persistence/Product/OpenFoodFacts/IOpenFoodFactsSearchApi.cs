using System.Text.Json.Serialization;
using Refit;

namespace ProperNutritionDiary.Product.Persistence.Product.OpenFoodFacts;

public interface IOpenFoodFactsSearchApi
{
    [Get("/cgi/search.pl?json=1&search_simple=1&search_terms={searchTerms}&page=1&page_size=200")]
    public Task<ApiResponse<OpenApiSearchResponse>> Search(string searchTerms, int page = 1);
}

public class OpenApiSearchResponse
{
    public class OpenApiSearchProduct
    {
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;
        public Nutrients Nutriments { get; set; } = default!;
    }

    public int Page { get; set; }

    [JsonPropertyName("product_name")]
    public int PageCount { get; set; }
    public OpenApiSearchProduct[] Products { get; set; } = [];
}
