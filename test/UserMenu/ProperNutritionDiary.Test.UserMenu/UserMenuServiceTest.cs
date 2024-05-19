using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProperNutritionDiary.UserMenuApi.Db;
using ProperNutritionDiary.UserMenuApi.Product.Entity;
using ProperNutritionDiary.UserMenuApi.UserMenu;
using ProperNutritionDiary.UserMenuApi.UserMenu.Create;
using ProperNutritionDiary.UserMenuApi.UserMenu.Create.Generate;
using ProperNutritionDiary.UserMenuApi.UserMenu.Edamam;
using Refit;
using Serilog;
using Xunit.Abstractions;

namespace ProperNutritionDiary.Test.UserMenu;

public class UserMenuServiceTest
{
    public IEdamamMenuApi edamamMenuApi;
    public IEdamamRecipeApi edamamRecipeApi;
    public EdamamConverter edamamConverter;
    private readonly UserMenuService userMenuService;
    private readonly ILogger<UserMenuServiceTest> logger;

    public UserMenuServiceTest(ITestOutputHelper output)
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

        var edamamMenuApiKey = Environment.GetEnvironmentVariable(
            "PROPER_NUTRITION_DIARY__USER_MENU_EDAMAM_MENU_API_KEY"
        );
        var edamamRecipeApiKey = Environment.GetEnvironmentVariable(
            "PROPER_NUTRITION_DIARY__USER_MENU_EDAMAM_RECIPE_API_KEY"
        );
        var edamamUserId = Environment.GetEnvironmentVariable(
            "PROPER_NUTRITION_DIARY__EDAMAM_USER_ID"
        );

        services
            .AddRefitClient<IEdamamMenuApi>(
                new RefitSettings() { ContentSerializer = new NewtonsoftJsonContentSerializer() }
            )
            .ConfigureHttpClient(cc =>
            {
                cc.BaseAddress = new Uri("https://api.edamam.com/api/meal-planner/v1");
                cc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    edamamMenuApiKey
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
                cc.DefaultRequestHeaders.Add("Edamam-Account-User", edamamUserId);
            });

        services.AddSingleton<EdamamConverter>();
        services.AddScoped<UserMenuService>();
        services.AddScoped<MenuConfigurationService>();
        services.AddDbContext<AppCtx>(conf =>
        {
            conf.UseInMemoryDatabase("Aboba");
        });

        var provider = services.BuildServiceProvider();

        edamamMenuApi = provider.GetRequiredService<IEdamamMenuApi>();
        edamamRecipeApi = provider.GetRequiredService<IEdamamRecipeApi>();
        edamamConverter = provider.GetRequiredService<EdamamConverter>();
        logger = provider.GetRequiredService<ILogger<UserMenuServiceTest>>();
        userMenuService = provider.GetRequiredService<UserMenuService>();
    }

    [Fact]
    public async Task TestCreatingMenu()
    {
        logger.LogInformation("Start creating");
        var res = await userMenuService.CreateFromEdamam(
            Guid.NewGuid(),
            new GenerateMenuConfiguration()
            {
                DayCount = 7,
                Health = [HealthType.AlcoholFree],
                Diet = [DietType.HighProtein],
                NutrientFilter = new() { TargetCalories = 3000, },
                Breakfast = new() { NutrientFilter = new() { TargetCalories = 500, } },
                Lunch = new() { NutrientFilter = new() { TargetCalories = 1500, } },
                Dinner = new() { NutrientFilter = new() { TargetCalories = 1000, } },
            }
        );
        logger.LogInformation("Result: {@Result}", res.IsSuccess ? res.Value : res.Error);

        logger.LogInformation(
            "End menu items: \n{@UserMenuItems}",
            string.Join(
                "\n",
                res.Value.DailyMenus.Select(x =>
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
                res.Value.DailyMenus.Select(x =>
                {
                    var sum = x.GetTotalMacronutrients();
                    return $"{sum.Calories:F2} {sum.Proteins:F2} {sum.Fats:F2} {sum.Carbohydrates:F2}";
                })
            )
        );
    }
}
