using System.Security.Cryptography.X509Certificates;
using DomainDesignLib.Abstractions.Result;

namespace ProperNutritionDiary.Product.Domain.Macronutrients;

public record Macronutrients
{
    private Macronutrients(double calories, double proteins, double fats, double carbohydrates)
    {
        Calories = calories;
        Proteins = proteins;
        Fats = fats;
        Carbohydrates = carbohydrates;
    }

    public static Result<Macronutrients> Create(
        double calories,
        double proteins,
        double fats,
        double carbohydrates
    )
    {
        return Result
            .Check(calories, CheckValueByNotLessZero, MacronutrientsErrors.ValueLessZero)
            .Check(proteins, CheckValueByNotLessZero, MacronutrientsErrors.ValueLessZero)
            .Check(fats, CheckValueByNotLessZero, MacronutrientsErrors.ValueLessZero)
            .Check(carbohydrates, CheckValueByNotLessZero, MacronutrientsErrors.ValueLessZero)
            .Success(() => new Macronutrients(calories, proteins, fats, carbohydrates));
    }

    private static bool CheckValueByNotLessZero(double value) => value < 0;

    public double Calories { get; init; }
    public double Proteins { get; init; }
    public double Fats { get; init; }
    public double Carbohydrates { get; init; }

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

    public static Macronutrients operator -(Macronutrients left, double right)
    {
        return new(
            left.Calories - right,
            left.Proteins - right,
            left.Fats - right,
            left.Carbohydrates - right
        );
    }

    public static Macronutrients operator +(Macronutrients left, double right)
    {
        return new(
            left.Calories + right,
            left.Proteins + right,
            left.Fats + right,
            left.Carbohydrates + right
        );
    }

    public static Macronutrients operator *(Macronutrients left, double right)
    {
        return new(
            left.Calories * right,
            left.Proteins * right,
            left.Fats * right,
            left.Carbohydrates * right
        );
    }

    public static Macronutrients operator /(Macronutrients left, double right)
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
}
