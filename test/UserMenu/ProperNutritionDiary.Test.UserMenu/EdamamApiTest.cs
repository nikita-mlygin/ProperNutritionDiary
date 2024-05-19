namespace ProperNutritionDiary.Test.UserMenu;

using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProperNutritionDiary.UserMenuApi.Product.Entity;
using ProperNutritionDiary.UserMenuApi.UserMenu.Edamam;
using Refit;
using Serilog;
using Xunit.Abstractions;

public class EdamamApiTest
{
    public IEdamamMenuApi edamamMenuApi;
    public IEdamamRecipeApi edamamRecipeApi;
    public EdamamConverter edamamConverter;
    private readonly ILogger<EdamamApiTest> logger;

    public EdamamApiTest(ITestOutputHelper output)
    {
        var services = new ServiceCollection();

        #region loggingConfig
        services.AddLogging(
            (builder) =>
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.TestOutput(output)
                    .CreateLogger();

                builder.AddSerilog();
            }
        );

        #endregion

        services
            .AddRefitClient<IEdamamMenuApi>(
                new RefitSettings() { ContentSerializer = new NewtonsoftJsonContentSerializer() }
            )
            .ConfigureHttpClient(cc =>
            {
                cc.BaseAddress = new Uri("https://api.edamam.com/api/meal-planner/v1");
                cc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    "NzkyZTk0ZTY6YTJiMGU1OTY1N2NmOTE1NjU3MDlkNjI0N2I4ZTFiMDM="
                );
                cc.DefaultRequestHeaders.Add("Edamam-Account-User", "792e94e6");
            });

        services
            .AddRefitClient<IEdamamRecipeApi>(
                new RefitSettings() { ContentSerializer = new NewtonsoftJsonContentSerializer() }
            )
            .ConfigureHttpClient(cc =>
            {
                cc.BaseAddress = new Uri("https://api.edamam.com/api/recipes/v2");
                cc.DefaultRequestHeaders.Add("Edamam-Account-User", "792e94e6");
            });

        services.AddSingleton<EdamamConverter>();

        var provider = services.BuildServiceProvider();

        edamamMenuApi = provider.GetRequiredService<IEdamamMenuApi>();
        edamamRecipeApi = provider.GetRequiredService<IEdamamRecipeApi>();
        edamamConverter = provider.GetRequiredService<EdamamConverter>();
        logger = provider.GetRequiredService<ILogger<EdamamApiTest>>();
    }

    [Fact]
    public async Task TestName()
    {
        var rqBody = new EdamamMenuRequest()
        {
            Size = 7,
            Plan = new()
            {
                Accept = new()
                {
                    All =
                    [
                        new EdamamMenuRequest.Predicate()
                        {
                            Health = ["SOY_FREE", "FISH_FREE", "MEDITERRANEAN"]
                        }
                    ]
                },
                Fit = new()
                {
                    ["ENERC_KCAL"] = new EdamamMenuRequest.Range() { Max = 2000, Min = 1000 },
                    ["SUGAR.added"] = new EdamamMenuRequest.Range() { Max = 20 },
                },
                Sections = new()
                {
                    ["Breakfast"] = new()
                    {
                        Accept = new()
                        {
                            All =
                            [
                                new()
                                {
                                    Dish =
                                    [
                                        "drinks",
                                        "egg",
                                        "biscuits and cookies",
                                        "bread",
                                        "pancake",
                                        "cereals"
                                    ],
                                },
                                new() { Meal = ["breakfast"], }
                            ],
                        },
                        Fit = new()
                        {
                            ["ENERC_KCAL"] = new EdamamMenuRequest.Range() { Max = 600, Min = 100 },
                        }
                    },
                    ["Lunch"] = new()
                    {
                        Accept = new()
                        {
                            All =
                            [
                                new()
                                {
                                    Dish =
                                    [
                                        "main course",
                                        "pasta",
                                        "egg",
                                        "salad",
                                        "soup",
                                        "sandwiches",
                                        "pizza",
                                        "seafood"
                                    ],
                                },
                                new() { Meal = ["lunch/dinner"], }
                            ],
                        },
                        Fit = new()
                        {
                            ["ENERC_KCAL"] = new EdamamMenuRequest.Range() { Max = 900, Min = 300 },
                        },
                        Sections = new()
                        {
                            ["Starter"] = new(),
                            ["Main"] = new(),
                            ["Dessert"] = new(),
                        }
                    },
                    ["Dinner"] = new()
                    {
                        Accept = new()
                        {
                            All =
                            [
                                new()
                                {
                                    Dish =
                                    [
                                        "seafood",
                                        "egg",
                                        "salad",
                                        "pizza",
                                        "pasta",
                                        "main course"
                                    ],
                                },
                                new() { Meal = ["lunch/dinner"] }
                            ],
                        },
                        Fit = new()
                        {
                            ["ENERC_KCAL"] = new EdamamMenuRequest.Range() { Max = 900, Min = 200 },
                        },
                        Sections = new() { ["Main"] = new(), ["Dessert"] = new(), }
                    }
                },
            },
        };

        var rqBody2 = new EdamamMenuRequest()
        {
            Size = 7,
            Plan = new()
            {
                Accept = new()
                {
                    All =
                    [
                        new EdamamMenuRequest.Predicate() { Health = ["FISH_FREE"] },
                        new EdamamMenuRequest.Predicate() { Diet = ["HIGH_PROTEIN"] }
                    ]
                },
                Fit = new()
                {
                    ["ENERC_KCAL"] = new EdamamMenuRequest.Range() { Max = 3000, Min = 2000 },
                    ["PROCNT"] = new EdamamMenuRequest.Range() { Max = 500, Min = 300 },
                },
                Sections = new()
                {
                    ["Breakfast"] = new()
                    {
                        Accept = new() { All = [new() { Meal = ["breakfast"], },], },
                        Fit = new()
                        {
                            ["ENERC_KCAL"] = new EdamamMenuRequest.Range() { Max = 900, Min = 200 },
                        }
                    },
                    ["Lunch"] = new()
                    {
                        Accept = new() { All = [new() { Meal = ["lunch/dinner"], }], },
                        Fit = new()
                        {
                            ["ENERC_KCAL"] = new EdamamMenuRequest.Range()
                            {
                                Max = 1350,
                                Min = 600
                            },
                        }
                    },
                    ["Dinner"] = new()
                    {
                        Accept = new() { All = [new() { Meal = ["lunch/dinner"] },], },
                        Fit = new()
                        {
                            ["ENERC_KCAL"] = new EdamamMenuRequest.Range()
                            {
                                Max = 1350,
                                Min = 400
                            },
                        },
                    }
                },
            },
        };

        var rqBodyStr = JsonConvert.SerializeObject(rqBody2);

        // Given
        var res = await edamamMenuApi.GetMenu("792e94e6", rqBody2);

        string aboba(string prev, KeyValuePair<string, EdamamMenuResponse.SelectionType.Section> x)
        {
            prev +=
                "\n"
                + x.Key
                + " "
                + (
                    x.Value.Assigned is null
                        ? x.Value.Sections.Aggregate(":", aboba)
                        : x.Value.Assigned
                );

            return prev;
        }

        logger.LogInformation(
            "Menu: {@Menu}",
            string.Join("\n", res.Content.Selection.Select(x => x.Sections.Aggregate("", aboba)))
        );

        res.Content.Should().NotBeNull();

        res.Content.Status.Should().Be("OK");

        var links = res.Content.Selection.SelectMany(x => GetLinks(x.Sections)).ToList();

        var recipeResultsApiResponse = (
            await Task.WhenAll(
                links
                    .Chunk(20)
                    .Select(x =>
                        edamamRecipeApi.GetByUrl(
                            [.. x.Take(20)],
                            "792e94e6",
                            "a2b0e59657cf91565709d6247b8e1b03",
                            [
                                "uri",
                                "label",
                                "yield",
                                "url",
                                "shareAs",
                                "ingredientLines",
                                "calories",
                                "totalWeight",
                                "totalNutrients"
                            ]
                        )
                    )
            )
        );

        logger.LogInformation(
            "recipeResultsApiResponse: {@recipeResultsApiResponse}",
            recipeResultsApiResponse
        );

        var recipeResults = recipeResultsApiResponse.SelectMany(x => x.Content.Hits).ToList();

        var menu = edamamConverter.Convert(recipeResults, res.Content, Guid.NewGuid());

        logger.LogInformation("RqBody: {RqBodyStr}", rqBodyStr);

        logger.LogInformation(
            "Menu: {@Menu}",
            string.Join("\n", res.Content.Selection.Select(x => x.Sections.Aggregate("", aboba)))
        );

        logger.LogInformation(
            "Recipes: {recipes}",
            string.Join(
                "\n",
                recipeResults.Select(x =>
                    $"{x.Recipe.Uri} {x.Recipe.TotalNutrients["ENERC_KCAL"].Quantity} {x.Recipe.TotalNutrients["PROCNT"].Quantity} {x.Recipe.TotalNutrients["FAT"].Quantity} {x.Recipe.TotalNutrients["CHOCDF"].Quantity}"
                )
            )
        );

        logger.LogInformation(
            "End menu items: \n{@UserMenuItems}",
            string.Join(
                "\n",
                menu.DailyMenus.Select(x =>
                    $"{x.DayNumber}d:\n"
                    + string.Join(
                        "\n",
                        x.MenuItems.GroupBy(x => x.ConsumptionNumber)
                            .Select(x =>
                                $"  [{x.Key}]:"
                                + string.Join(
                                    "\n",
                                    x.Select(x =>
                                        $"\t * {x.ProductName} ({x.Weight}g): {(x.ProductId as EdamamProductIdentity)!.Url}\n\t ({x.RecipeUrl})\n\tCPFC: {x.Macronutrients.Calories:F2} {x.Macronutrients.Proteins:F2} {x.Macronutrients.Fats:F2} {x.Macronutrients.Carbohydrates:F2} => {x.Macronutrients.Calories * x.Weight / 100:F2} {x.Macronutrients.Proteins * x.Weight / 100:F2} {x.Macronutrients.Fats * x.Weight / 100:F2} {x.Macronutrients.Carbohydrates * x.Weight / 100:F2} \n{string.Join("\n", x.RecipeLines?.Select(x => $"\t => {x}") ?? [])}"
                                    )
                                )
                            )
                    )
                )
            )
        );

        logger.LogInformation(
            "End menu items: \n{@UserMenuItems}",
            string.Join(
                "\n",
                menu.DailyMenus.Select(x =>
                {
                    var sum = x.GetTotalMacronutrients();
                    return $"{sum.Calories:F2} {sum.Proteins:F2} {sum.Fats:F2} {sum.Carbohydrates:F2}";
                })
            )
        );
    }

    private static IEnumerable<string> GetLinks(
        Dictionary<string, EdamamMenuResponse.SelectionType.Section> sections
    )
    {
        return sections.SelectMany(x =>
            x.Value.Assigned is not null ? [x.Value.Assigned] : GetLinks(x.Value.Sections!)
        );
    }
}
