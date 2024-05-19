namespace ProperNutritionDiary.UserMenuApi.UserMenu.Edamam;

using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using ProperNutritionDiary.UserMenuApi.Product.Entity;
using ProperNutritionDiary.UserMenuApi.UserMenu.Entity;

public class EdamamConverter(ILogger<EdamamConverter> logger)
{
    public UserMenuItem Convert(EdamamRecipeResponse.Hit.RecipeType response, int consumptionNumber)
    {
        logger.LogInformation("Convert start user item start: {@Product}", response);

        return new UserMenuItem()
        {
            Id = Guid.NewGuid(),
            ConsumptionNumber = consumptionNumber,
            Macronutrients =
                Macronutrients
                    .Create(
                        response.TotalNutrients["ENERC_KCAL"].Quantity,
                        response.TotalNutrients["PROCNT"].Quantity,
                        response.TotalNutrients["FAT"].Quantity,
                        response.TotalNutrients["CHOCDF"].Quantity
                    )
                    .Value
                / response.TotalWeight
                * 100,
            Weight = response.TotalWeight / response.Yield,
            ProductId = new EdamamProductIdentity(response.Uri),
            ProductName = response.Label,
            RecipeLines = response.IngredientLines,
            RecipeUrl = response.Url,
        };
    }

    public UserMenu Convert(
        List<EdamamRecipeResponse.Hit> hits,
        EdamamMenuResponse menu,
        Guid userId
    ) =>
        new()
        {
            DailyMenus = CreateFromSections(menu.Selection, hits).ToList(),
            Date = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            UserId = userId
        };

    private IEnumerable<UserDailyMenu> CreateFromSections(
        EdamamMenuResponse.SelectionType[] section,
        List<EdamamRecipeResponse.Hit> hits
    )
    {
        int i = 0;

        return section.Select(
            (x, dn) =>
                new UserDailyMenu()
                {
                    DayNumber = dn,
                    Id = Guid.NewGuid(),
                    MenuItems =
                    [
                        .. x.Sections!.Select(
                        (x, cn) =>
                        {
                            var res = CreateFromSections(
                                x.Value,
                                hits,
                                i,
                                x.Key switch
                                {
                                    "Breakfast" => 1,
                                    "Lunch" => 2,
                                    "Dinner" => 3,
                                    _ => cn + 1
                                }
                            );

                            i += res.Count();

                            return res;
                        }
                    )
                        .SelectMany(x => x)
                        .OrderBy(x => x.ConsumptionNumber)
                    ]
                }
        );
    }

    private IEnumerable<UserMenuItem> CreateFromSections(
        EdamamMenuResponse.SelectionType.Section section,
        List<EdamamRecipeResponse.Hit> hits,
        int i,
        int cn
    )
    {
        return section.Assigned is not null
            ? [Convert(hits[i].Recipe, cn)]
            : section
                .Sections!.Select((x, ia) => CreateFromSections(x.Value, hits, i + ia, cn))
                .SelectMany(x => x);
    }

    public IEnumerable<string> GetLinks(
        Dictionary<string, EdamamMenuResponse.SelectionType.Section> sections
    )
    {
        logger.LogInformation("Get links started");

        return sections.SelectMany(x =>
            x.Value.Assigned is not null ? [x.Value.Assigned] : GetLinks(x.Value.Sections!)
        );
    }
}
