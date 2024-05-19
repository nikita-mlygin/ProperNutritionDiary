export interface NutrientFilterType {
    TargetCalories?: number;
    TargetProtein?: number;
    TargetFats?: number;
    TargetCarbohydrates?: number;
    Other?: { [key: string]: number };
}

export interface SectionMenuFilter extends BaseGenerateMenuConfiguration {
    MealType: string[];
}

export interface BaseGenerateMenuConfiguration {
    NutrientFilter?: NutrientFilterType;
    Dish: string[];
}

export interface GenerateMenuConfiguration extends BaseGenerateMenuConfiguration {
    Breakfast?: SectionMenuFilter;
    Lunch?: SectionMenuFilter;
    Dinner?: SectionMenuFilter;
    Cuisine: string[];
    Diet: string[];
    Health: string[];
    DayCount: number;
}
