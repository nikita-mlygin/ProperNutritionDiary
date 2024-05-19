namespace ProperNutritionDiary.UserMenuApi.UserMenu.Create;

public class BaseGenerateMenuConfiguration
{
    public class SectionMenuFilter : BaseGenerateMenuConfiguration
    {
        public string[] MealType { get; set; } = [];
    }

    public class NutrientFilterType
    {
        public decimal? TargetCalories { get; set; }
        public decimal? TargetProtein { get; set; }
        public decimal? TargetFats { get; set; }
        public decimal? TargetCarbohydrates { get; set; }

        public Dictionary<string, decimal> Other { get; set; } = [];
    }

    public NutrientFilterType? NutrientFilter { get; set; }
    public string[] Dish { get; set; } = [];
}

public class GenerateMenuConfiguration : BaseGenerateMenuConfiguration
{
    public SectionMenuFilter? Breakfast { get; set; }
    public SectionMenuFilter? Lunch { get; set; }
    public SectionMenuFilter? Dinner { get; set; }
    public string[] Cuisine { get; set; } = [];
    public string[] Diet { get; set; } = [];
    public string[] Health { get; set; } = [];
    public int DayCount { get; set; }
}
