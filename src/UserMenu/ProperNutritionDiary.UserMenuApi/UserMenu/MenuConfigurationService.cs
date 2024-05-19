using DomainDesignLib.Abstractions.Result;
using Newtonsoft.Json;
using ProperNutritionDiary.UserMenuApi.UserMenu.Create;
using ProperNutritionDiary.UserMenuApi.UserMenu.Create.Generate;
using ProperNutritionDiary.UserMenuApi.UserMenu.Edamam;

namespace ProperNutritionDiary.UserMenuApi.UserMenu;

public class MenuConfigurationService(ILogger<MenuConfigurationService> logger)
{
    public Result<EdamamMenuRequest> Create(GenerateMenuConfiguration cfg)
    {
        EdamamMenuRequest.MainPredicate mainAccept = CreateFrom(cfg);
        var mainFit = CreateFit(cfg, 500);

        if (cfg.Health.Length != 0)
            mainAccept.All!.Add(new EdamamMenuRequest.Predicate() { Health = cfg.Health });

        if (cfg.Cuisine.Length != 0)
            mainAccept.All!.Add(new EdamamMenuRequest.Predicate() { Cuisine = cfg.Cuisine });

        if (cfg.Diet.Length != 0)
            mainAccept.All!.Add(new EdamamMenuRequest.Predicate() { Diet = cfg.Diet });

        var breakfastAccept = cfg.Breakfast is not null
            ? CreateFrom(cfg.Breakfast)
            : new EdamamMenuRequest.MainPredicate() { All = [] };

        breakfastAccept.All!.Add(
            new EdamamMenuRequest.Predicate() { Meal = [MealTypes.breakfast] }
        );

        var breakfastFit = cfg.Breakfast is not null ? CreateFit(cfg.Breakfast, 1000) : null;

        var lunchAccept = cfg.Lunch is not null
            ? CreateFrom(cfg.Lunch)
            : new EdamamMenuRequest.MainPredicate() { All = [] };

        lunchAccept.All!.Add(new EdamamMenuRequest.Predicate() { Meal = [MealTypes.lunchDinner] });

        var lunchFit = cfg.Lunch is not null ? CreateFit(cfg.Lunch, 1000) : null;

        var dinnerAccept = cfg.Dinner is not null
            ? CreateFrom(cfg.Dinner)
            : new EdamamMenuRequest.MainPredicate() { All = [] };

        dinnerAccept.All!.Add(new EdamamMenuRequest.Predicate() { Meal = [MealTypes.lunchDinner] });

        var dinnerFit = cfg.Dinner is not null ? CreateFit(cfg.Dinner, 1000) : null;

        logger.LogInformation(
            "Nutrient filter: {@MainFit}, main accept generated: {@MainAccept}",
            mainFit,
            mainAccept
        );

        var rqBody = new EdamamMenuRequest()
        {
            Size = cfg.DayCount,
            Plan = new()
            {
                Accept = mainAccept,
                Fit = mainFit,
                Sections = new()
                {
                    ["Breakfast"] = new() { Accept = breakfastAccept, Fit = breakfastFit, },
                    ["Lunch"] = new()
                    {
                        Accept = lunchAccept,
                        Fit = lunchFit,
                        Sections = new()
                        {
                            ["Starter"] = new(),
                            ["Main"] = new(),
                            ["Dessert"] = new(),
                        }
                    },
                    ["Dinner"] = new()
                    {
                        Accept = dinnerAccept,
                        Fit = dinnerFit,
                        Sections = new() { ["Main"] = new(), ["Dessert"] = new(), }
                    }
                },
            },
        };

        logger.LogInformation(
            "Request body generated: {@RqBody}, JsonString: {JsonString}",
            rqBody,
            JsonConvert.SerializeObject(rqBody)
        );

        return Result.Success(rqBody);
    }

    private EdamamMenuRequest.MainPredicate CreateFrom(BaseGenerateMenuConfiguration cfg)
    {
        EdamamMenuRequest.MainPredicate mainAccept = new() { All = [] };

        if (cfg.Dish.Length != 0)
            mainAccept.All.Add(new EdamamMenuRequest.Predicate() { Dish = cfg.Dish });

        return mainAccept;
    }

    private Dictionary<string, EdamamMenuRequest.Range> CreateFit(
        BaseGenerateMenuConfiguration cfg,
        decimal? accuracyI = null
    )
    {
        Dictionary<string, EdamamMenuRequest.Range> mainFit = [];

        var accuracy = accuracyI ?? 500;

        if (cfg.NutrientFilter is not null)
        {
            if (cfg.NutrientFilter.TargetCalories is not null)
            {
                mainFit.Add(
                    NutrientType.Energy,
                    new EdamamMenuRequest.Range()
                    {
                        Min = decimal.ToInt32(
                            cfg.NutrientFilter.TargetCalories.Value - accuracy / 2
                        ),
                        Max = decimal.ToInt32(
                            cfg.NutrientFilter.TargetCalories.Value + accuracy / 2
                        ),
                    }
                );
            }

            if (cfg.NutrientFilter.TargetProtein is not null)
            {
                mainFit.Add(
                    NutrientType.Protein,
                    new EdamamMenuRequest.Range()
                    {
                        Min = decimal.ToInt32(
                            cfg.NutrientFilter.TargetProtein.Value - accuracy / 2
                        ),
                        Max = decimal.ToInt32(
                            cfg.NutrientFilter.TargetProtein.Value + accuracy / 2
                        ),
                    }
                );
            }

            if (cfg.NutrientFilter.TargetFats is not null)
            {
                mainFit.Add(
                    NutrientType.Fat,
                    new EdamamMenuRequest.Range()
                    {
                        Min = decimal.ToInt32(cfg.NutrientFilter.TargetFats.Value - accuracy / 2),
                        Max = decimal.ToInt32(cfg.NutrientFilter.TargetFats.Value + accuracy / 2),
                    }
                );
            }

            if (cfg.NutrientFilter.TargetCarbohydrates is not null)
            {
                mainFit.Add(
                    NutrientType.Carbohydrate,
                    new EdamamMenuRequest.Range()
                    {
                        Min = decimal.ToInt32(
                            cfg.NutrientFilter.TargetCarbohydrates.Value - accuracy / 2
                        ),
                        Max = decimal.ToInt32(
                            cfg.NutrientFilter.TargetCarbohydrates.Value + accuracy / 2
                        ),
                    }
                );
            }

            if (cfg.NutrientFilter.Other is not null)
            {
                foreach (var item in cfg.NutrientFilter.Other)
                {
                    mainFit.Add(
                        item.Key,
                        new EdamamMenuRequest.Range()
                        {
                            Min = decimal.ToInt32(item.Value - accuracy / 2),
                            Max = decimal.ToInt32(item.Value + accuracy / 2),
                        }
                    );
                }
            }
        }

        return mainFit;
    }
}
