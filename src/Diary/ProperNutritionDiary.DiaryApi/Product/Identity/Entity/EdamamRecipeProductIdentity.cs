using ProperNutritionDiary.DiaryApi.Product.Identity;
using ProperNutritionDiary.DiaryContracts.Product;

namespace ProperNutritionDiary.DiaryApi.Product.Identity.Entity;

public class EdamamRecipeProductIdentity(string uri) : ProductIdentity
{
    public string Uri { get; set; } = uri;

    public override SourceType Type
    {
        get => SourceType.EdamamRecipe;
    }
}
