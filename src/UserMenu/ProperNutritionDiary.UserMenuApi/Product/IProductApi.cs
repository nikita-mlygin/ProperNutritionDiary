using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;
using Refit;

namespace ProperNutritionDiary.UserMenuApi.Product;

public interface IProductApi
{
    [Post("/product")]
    public Task<Guid> CreateProduct(CreateProductRequest rq);

    [Get("/product/{id}")]
    public Task<ProductDetails> GetProductById([AliasAs("id")] Guid id);
}

public class ProductDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public Macronutrients Macronutrients { get; set; } = default!;
    public Guid? Owner { get; set; }
    public int ViewCount { get; set; }
    public int UseCount { get; set; }
}

public class CreateProductRequest
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Calories { get; set; }
    public decimal Proteins { get; set; }
    public decimal Fats { get; set; }
    public decimal Carbohydrates { get; set; }
}
