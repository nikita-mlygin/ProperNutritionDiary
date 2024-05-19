namespace ProperNutritionDiary.UserMenuApi.UserMenu;

using System.Text.Json;
using DomainDesignLib.Abstractions;
using DomainDesignLib.Abstractions.Result;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.UserMenuApi.Db;
using ProperNutritionDiary.UserMenuApi.Product;
using ProperNutritionDiary.UserMenuApi.Product.Entity;
using ProperNutritionDiary.UserMenuApi.UserMenu.Create;
using ProperNutritionDiary.UserMenuApi.UserMenu.Edamam;
using ProperNutritionDiary.UserMenuApi.UserMenu.Entity;
using ProperNutritionDiary.UserMenuApi.UserMenu.Get.Details;
using Refit;

public class UserMenuService(
    AppCtx ctx,
    ILogger<UserMenuService> logger,
    IEdamamMenuApi edamamMenuApi,
    IEdamamRecipeApi edamamRecipeApi,
    MenuConfigurationService menuConfigurationService,
    EdamamConverter edamamConverter
)
{
    public Task<Result<Guid>> Create(
        Guid id,
        Guid userId,
        string role,
        List<Dictionary<int, CreateMenuItem>> items
    )
    {
        var dailies = items.Select(
            (x, i) =>
            {
                var items = x.Select(
                    (x) =>
                    {
                        (var dayNumber, var itemSnapshot) = x;

                        var item = itemSnapshot.Adapt<UserMenuItem>();
                        item.Id = Guid.NewGuid();
                        item.ConsumptionNumber = dayNumber;

                        item.Macronutrients = Macronutrients.Create(0, 0, 0, 0).Value;

                        item.ProductId = itemSnapshot.Product.Type switch
                        {
                            ProductIdentityType.SystemItem
                                => new SystemProductIdentity()
                                {
                                    Id = Guid.NewGuid(),
                                    Guid = Guid.Parse(itemSnapshot.Product.Value)
                                },
                            ProductIdentityType.Barcode
                                => new BarcodeProductIdentity()
                                {
                                    Id = Guid.NewGuid(),
                                    Barcode = itemSnapshot.Product.Value,
                                },
                            ProductIdentityType.USDA
                                => new UsdaProductIdentity()
                                {
                                    Id = Guid.NewGuid(),
                                    Code = itemSnapshot.Product.Value,
                                },
                            _ => throw new ArgumentException("Invalid product identity type")
                        };

                        // TODO
                        item.ProductName = "GET NAME";

                        return item;
                    }
                );

                return new UserDailyMenu()
                {
                    Id = Guid.NewGuid(),
                    DayNumber = i,
                    MenuItems = items.ToList()
                };
            }
        );

        return Result
            .Check(role != "plain", new Error("CreateMenuError", "Can't crate menu with this role"))
            .Success(async () =>
            {
                var menu = new UserMenu()
                {
                    Id = Guid.NewGuid(),
                    DailyMenus = dailies.ToList(),
                    Date = DateTime.UtcNow,
                    UserId = userId
                };

                await ctx.AddAsync(menu);

                await ctx.SaveChangesAsync();

                return menu.Id;
            });
    }

    public async Task<Result<Details>> GetActualMenu(Guid userId, DateTime date)
    {
        var actualMenu = await ctx
            .UserMenus.OrderBy(x => x.Date)
            .Take(1)
            .Where(x => x.UserId == userId && x.Date <= date)
            .Include(x => x.DailyMenus)
            .ThenInclude(x => x.MenuItems)
            .ThenInclude(x => x.ProductId)
            .FirstOrDefaultAsync();

        logger.LogInformation(
            "Actual menu received: @{ActualMenu}",
            JsonSerializer.Serialize(actualMenu)
        );

        return Result
            .Check(actualMenu is null, new Error("UserMenuError", "User have not actual menu"))
            .Success(Adapt(actualMenu!));
    }

    public Task<Result<UserMenu>> CreateFromEdamam(Guid userId, GenerateMenuConfiguration cfg)
    {
        var menuCfg = menuConfigurationService.Create(cfg);
        EdamamMenuResponse? response = null;
        Result<EdamamRecipeResponse[]> recipeResponses = Result.Failure<EdamamRecipeResponse[]>(
            new Error("UserMenu", "Recipe results is null")
        );

        return Result
            .Check(menuCfg.IsFailure, menuCfg.Error)
            .Check(
                () => menuCfg.Value,
                async (menu) =>
                    (response = await CreateResponse(menu)).Match(
                        _ => Result.Success(),
                        () => Result.Failure(new Error("UserMenu", "Cannot get menu"))
                    )
            )
            .Check(
                () => response,
                async (response) =>
                    (recipeResponses = await GetRecipes(response!)).Match(
                        _ => Result.Success(),
                        err => Result.Failure(err)
                    )
            )
            .Success(() => CreateUserMenu(response!, recipeResponses.Value, userId))
            .Build();
    }

    private async Task<EdamamMenuResponse?> CreateResponse(EdamamMenuRequest rq)
    {
        var response = await edamamMenuApi.GetMenu("792e94e6", rq);
        if (response.Content is null || response.Error is not null)
            logger.LogInformation("Error while getting menu with message {@Error}", response.Error);
        else
            logger.LogInformation("UserMenuResponseContent: {@Content}", response.Content);

        return response.Content;
    }

    private async Task<Result<EdamamRecipeResponse[]>> GetRecipes(EdamamMenuResponse rs)
    {
        var links = rs.Selection.SelectMany(x => GetLinks(x.Sections));

        var recipeResultsApiResponse = (
            await Task.WhenAll(
                links
                    .Chunk(20)
                    .Select(x =>
                        edamamRecipeApi.GetByUrl(
                            [.. x.Take(20)],
                            "792e94e6",
                            Environment.GetEnvironmentVariable(
                                "PROPER_NUTRITION_DIARY__USER_MENU_EDAMAM_RECIPE_API_KEY"
                            ) ?? throw new Exception(),
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
        ).Select(x => x.Content);

        if (recipeResultsApiResponse.Any(x => x is null))
        {
            return Result.Failure<EdamamRecipeResponse[]>(
                new Error("UserMenu", "Error while getting recipes")
            );
        }

        return Result.Success(recipeResultsApiResponse.Select(x => x!).ToArray());
    }

    private UserMenu CreateUserMenu(
        EdamamMenuResponse menuRs,
        EdamamRecipeResponse[] recipesRs,
        Guid userId
    )
    {
        return edamamConverter.Convert(recipesRs.SelectMany(x => x.Hits).ToList(), menuRs, userId);
    }

    private static IEnumerable<string> GetLinks(
        Dictionary<string, EdamamMenuResponse.SelectionType.Section> sections
    )
    {
        return sections.SelectMany(x =>
            x.Value.Assigned is not null ? [x.Value.Assigned] : GetLinks(x.Value.Sections!)
        );
    }

    public static Func<Details> Adapt(UserMenu userMenu)
    {
        return () =>
        {
            var res = userMenu.Adapt<Details>();

            res.DailyMenus = userMenu
                .DailyMenus.Select(x =>
                {
                    var res = x.Adapt<DetailsDay>();

                    res.Breakfast = x
                        .MenuItems.Where(x => x.ConsumptionNumber == 1)
                        .Select(Adapt)
                        .ToList();

                    res.Lunch = x
                        .MenuItems.Where(x => x.ConsumptionNumber == 2)
                        .Select(Adapt)
                        .ToList();

                    res.Dinner = x
                        .MenuItems.Where(x => x.ConsumptionNumber == 3)
                        .Select(Adapt)
                        .ToList();

                    return res;
                })
                .ToList();

            return res;
        };
    }

    private static MenuItemDetails Adapt(UserMenuItem x)
    {
        var res = x.Adapt<MenuItemDetails>();

        (ProductIdentityType identityType, string value)? resProductId = x
            .ProductId.For(
                (SystemProductIdentity identity) =>
                {
                    return (ProductIdentityType.SystemItem, identity.Guid.ToString());
                }
            )
            .For(
                (UsdaProductIdentity identity) =>
                {
                    return (ProductIdentityType.USDA, identity.Code);
                }
            )
            .For(
                (BarcodeProductIdentity identity) =>
                {
                    return (ProductIdentityType.Barcode, identity.Barcode);
                }
            )
            .For(
                (EdamamProductIdentity identity) =>
                {
                    return (ProductIdentityType.Edamam, identity.Url);
                }
            );

        (ProductIdentityType identityType, string value) = resProductId ?? (default, "");

        res.ProductId = value;
        res.ProductIdentityType = identityType;

        return res;
    }
}
