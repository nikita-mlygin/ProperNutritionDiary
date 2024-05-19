using Refit;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Edamam;

public interface IEdamamRecipeApi
{
    [Get("/by-uri")]
    public Task<ApiResponse<EdamamRecipeResponse>> GetByUrl(
        [Query(CollectionFormat.Multi)] string[] uri,
        [AliasAs("app_id")] string appId,
        [AliasAs("app_key")] string appKey,
        [Query(CollectionFormat.Multi)] string[] field,
        string type = "public"
    );
}

public class EdamamRecipeResponse
{
    public class Hit
    {
        public class RecipeType
        {
            public class NutrientItem
            {
                public string Label { get; set; } = string.Empty;
                public decimal Quantity { get; set; }
                public string Unit { get; set; } = string.Empty;
            }

            public string Uri { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public decimal Yield { get; set; }
            public string Source { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string ShareAs { get; set; } = string.Empty;
            public List<string> IngredientLines { get; set; } = [];
            public decimal Calories { get; set; }
            public decimal TotalWeight { get; set; }
            public Dictionary<string, NutrientItem> TotalNutrients { get; set; } = [];
        }

        public RecipeType Recipe { get; set; } = default!;
    }

    public int From { get; set; }
    public int To { get; set; }
    public int Count { get; set; }
    public Hit[] Hits { get; set; } = [];
}
