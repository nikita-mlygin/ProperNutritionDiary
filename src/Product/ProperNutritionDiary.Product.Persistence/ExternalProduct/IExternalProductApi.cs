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
        public float ServingSize { get; set; }
        public string ServingSizeUnit { get; set; } = "g";
    }

    public class FoodWithRecipe : StandardFood
    {
        public string RecipeLink { get; set; }
    }

    [Get("/api/search")]
    Task<(List<StandardFood>, List<int>)> SearchFoodAsync(
        [Query] string q,
        [Query] int page = 1,
        [Query] string? source = null,
        [Query] float min_calories = 0,
        [Query] float max_calories = float.PositiveInfinity
    );

    [Get("/api")]
    Task<StandardFood> GetFoodAsync([Query] string id, [Query] string source);

    [Get("/api/recipe/s")]
    Task<(List<FoodWithRecipe>, string)> SearchEdamamRecipesAsync(
        [Query] string q,
        [Query] string? cont = null
    );

    [Get("/api/by-uri")]
    Task<List<FoodWithRecipe>> GetFoodFromEdamamAsync(
        [Query(CollectionFormat.Multi)] List<string> uri
    );
}
