using ProperNutritionDiary.UserMenuApi.Product;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

namespace ProperNutritionDiary.Product.Domain.Product.Identity.Entity;

public class EdamamRecipeProductIdentity(string uri) : ExternalSourceIdentity
{
    public string Uri { get; set; } = uri;

    public override ExternalSourceType Type
    {
        get => ExternalSourceType.EdamamRecipe;
    }
}
