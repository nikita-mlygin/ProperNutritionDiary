using DomainDesignLib.Abstractions.Result;
using Mapster;

namespace ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

public record Macronutrients
{
    /// <summary>
    /// Only for EF Core
    /// </summary>
    public Macronutrients() { }

    private Macronutrients(decimal calories, decimal proteins, decimal fats, decimal carbohydrates)
    {
        Calories = calories;
        Proteins = proteins;
        Fats = fats;
        Carbohydrates = carbohydrates;
    }

    public static Result<Macronutrients> Create(
        decimal calories,
        decimal proteins,
        decimal fats,
        decimal carbohydrates
    )
    {
        return Result
            .Check(calories, CheckValueByNotLessZero, MacronutrientsErrors.ValueLessZero)
            .Check(proteins, CheckValueByNotLessZero, MacronutrientsErrors.ValueLessZero)
            .Check(fats, CheckValueByNotLessZero, MacronutrientsErrors.ValueLessZero)
            .Check(carbohydrates, CheckValueByNotLessZero, MacronutrientsErrors.ValueLessZero)
            .Success(() => new Macronutrients(calories, proteins, fats, carbohydrates));
    }

    private static bool CheckValueByNotLessZero(decimal value) => value < 0;

    public decimal Calories { get; init; }
    public decimal Proteins { get; init; }
    public decimal Fats { get; init; }
    public decimal Carbohydrates { get; init; }

    public static Macronutrients operator +(Macronutrients left, Macronutrients right)
    {
        return new(
            left.Calories + right.Calories,
            left.Proteins + right.Proteins,
            left.Fats + right.Fats,
            left.Carbohydrates + right.Carbohydrates
        );
    }

    public static Macronutrients operator -(Macronutrients left, Macronutrients right)
    {
        return new(
            left.Calories - right.Calories,
            left.Proteins - right.Proteins,
            left.Fats - right.Fats,
            left.Carbohydrates - right.Carbohydrates
        );
    }

    public static Macronutrients operator -(Macronutrients left, decimal right)
    {
        return new(
            left.Calories - right,
            left.Proteins - right,
            left.Fats - right,
            left.Carbohydrates - right
        );
    }

    public static Macronutrients operator +(Macronutrients left, decimal right)
    {
        return new(
            left.Calories + right,
            left.Proteins + right,
            left.Fats + right,
            left.Carbohydrates + right
        );
    }

    public static Macronutrients operator *(Macronutrients left, decimal right)
    {
        return new(
            left.Calories * right,
            left.Proteins * right,
            left.Fats * right,
            left.Carbohydrates * right
        );
    }

    public static Macronutrients operator /(Macronutrients left, decimal right)
    {
        return new(
            left.Calories / right,
            left.Proteins / right,
            left.Fats / right,
            left.Carbohydrates / right
        );
    }

    public static Macronutrients FromSnapshot(MacronutrientsSnapshot snapshot)
    {
        return new(snapshot.Calories, snapshot.Proteins, snapshot.Fats, snapshot.Carbohydrates);
    }

    public MacronutrientsSnapshot ToSnapshot()
    {
        return this.Adapt<MacronutrientsSnapshot>();
    }
}
