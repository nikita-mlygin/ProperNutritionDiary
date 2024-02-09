using FluentAssertions;
using ProperNutritionDiary.Product.Domain.Macronutrients;

namespace ProperNutritionDiary.Test.Product.Test.Domain;

public class MacronutrientsTest
{
    private readonly Macronutrients left = Macronutrients.Create(200, 34, 21, 11).Value;
    private readonly Macronutrients right = Macronutrients.Create(90, 0, 12, 44).Value;

    [Fact]
    public void SumOfTwoMacronutrients_ShouldReturnSumOfLeftAndRight()
    {
        var sum = left + right;

        sum.Calories.Should().Be(left.Calories + right.Calories);
        sum.Proteins.Should().Be(left.Proteins + right.Proteins);
        sum.Fats.Should().Be(left.Fats + right.Fats);
        sum.Carbohydrates.Should().Be(left.Carbohydrates + right.Carbohydrates);
    }
}
