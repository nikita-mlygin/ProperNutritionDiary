using Refit;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Edamam;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public interface IEdamamMenuApi
{
    [Post("/{app_id}/select")]
    public Task<ApiResponse<EdamamMenuResponse>> GetMenu(
        [AliasAs("app_id")] string AppId,
        [Body] EdamamMenuRequest body
    );
}

[JsonObject(
    ItemNullValueHandling = NullValueHandling.Ignore,
    NamingStrategyType = typeof(CamelCaseNamingStrategy)
)]
public class EdamamMenuRequest
{
    [JsonObject(
        ItemNullValueHandling = NullValueHandling.Ignore,
        NamingStrategyType = typeof(CamelCaseNamingStrategy)
    )]
    public class Range
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    [JsonObject(
        ItemNullValueHandling = NullValueHandling.Ignore,
        NamingStrategyType = typeof(CamelCaseNamingStrategy)
    )]
    public class MainPredicate
    {
        public List<Predicate>? All { get; set; }
        public List<Predicate>? Any { get; set; }
        public List<Predicate>? Not { get; set; }
    }

    [JsonObject(
        ItemNullValueHandling = NullValueHandling.Ignore,
        NamingStrategyType = typeof(CamelCaseNamingStrategy)
    )]
    public class Predicate
    {
        public string[]? Dish { get; set; }
        public string[]? Meal { get; set; }
        public string[]? Health { get; set; }
        public string[]? Diet { get; set; }
        public string[]? Cuisine { get; set; }
    }

    [JsonObject(
        ItemNullValueHandling = NullValueHandling.Ignore,
        NamingStrategyType = typeof(CamelCaseNamingStrategy)
    )]
    public class MenuPlan
    {
        public MainPredicate? Accept { get; set; }
        public Dictionary<string, Range>? Fit { get; set; }
        public Dictionary<string, MenuPlan>? Sections { get; set; }
    }

    public int Size { get; set; }
    public MenuPlan Plan { get; set; } = default!;
}

public class EdamamMenuResponse
{
    public class SelectionType
    {
        public Dictionary<string, Section> Sections { get; set; } = [];

        public class Section
        {
            public string? Assigned { get; set; }
            public Dictionary<string, Section>? Sections { get; set; }
        }
    }

    public SelectionType[] Selection { get; set; } = [];

    public string Status { get; set; } = string.Empty;
}
