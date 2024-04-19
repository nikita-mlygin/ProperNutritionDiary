using DomainDesignLib.Abstractions.Result;

namespace ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

public static class MacronutrientsErrors
{
    public static readonly Error ValueLessZero =
        new($"{nameof(Macronutrients)}.{nameof(ValueLessZero)}", "Value must be not less null");
}
