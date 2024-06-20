using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace ProperNutritionDiary.Product.Persistence.ExternalProduct;

public interface IExternalProductApi
{
    public class StandardFood
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public Dictionary<string, decimal> Nutrients { get; set; } = [];
        public List<string> Ingredients { get; set; } = [];
        public List<string> Allergens { get; set; } = [];
        public decimal ServingSize { get; set; }
        public string ServingSizeUnit { get; set; } = "g";
    }

    public class FoodWithRecipe : StandardFood
    {
        public string RecipeLink { get; set; } = string.Empty;
    }

    public class SearchResult
    {
        public List<StandardFood> ProductList { get; set; } = [];
        public List<int> PageNumbers { get; set; } = [];
    }

    public class RecipeSearchResult
    {
        public List<FoodWithRecipe> ProductList { get; set; } = [];
        public string? Next { get; set; }
    }

    [Get("/api/search")]
    Task<SearchResult> SearchFoodAsync(
        [Query] string q,
        [Query] int page = 1,
        [Query] string? source = null
    );

    [Get("/api")]
    Task<StandardFood> GetFoodAsync([Query] string id, [Query] string source);

    [Get("/api/recipe/s")]
    Task<RecipeSearchResult> SearchEdamamRecipesAsync(
        [Query] string q,
        [Query] string? cont = null
    );

    [Get("/api/by-uri")]
    Task<List<FoodWithRecipe>> GetFoodFromEdamamAsync(
        [Query(CollectionFormat.Multi)] List<string> uri
    );
}
