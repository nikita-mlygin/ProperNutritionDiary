using ProperNutritionDiary.Product.Domain.Macronutrients;
using ProperNutritionDiary.Product.Domain.Product;

namespace ProperNutritionDiary.Product.Persistence.Product.Extensions;

internal static class ProductSnapshotExtensions
{
    public static Dictionary<string, object?> GetParam(this ProductSnapshot product)
    {
        return new Dictionary<string, object?>()
        {
            { nameof(ProductSnapshot.Id), product.Id },
            { nameof(ProductSnapshot.Name), product.Name },
            { nameof(ProductSnapshot.CreatedAt), product.CreatedAt },
            { nameof(ProductSnapshot.UpdatedAt), product.UpdatedAt },
            { nameof(ProductSnapshot.Owner), product.Owner },
            { nameof(MacronutrientsSnapshot.Calories), product.Macronutrients.Calories },
            { nameof(MacronutrientsSnapshot.Proteins), product.Macronutrients.Proteins },
            { nameof(MacronutrientsSnapshot.Fats), product.Macronutrients.Fats },
            { nameof(MacronutrientsSnapshot.Carbohydrates), product.Macronutrients.Carbohydrates },
        };
    }
}
