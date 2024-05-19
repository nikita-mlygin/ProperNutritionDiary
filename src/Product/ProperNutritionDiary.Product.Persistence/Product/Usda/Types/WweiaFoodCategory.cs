namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// WweiaFoodCategory:
//       properties:
//         wweiaFoodCategoryCode:
//           type: integer
//           example: 3002
//         wweiaFoodCategoryDescription:
//           type: string
//           example: "Meat mixed dishes"

public class WweiaFoodCategory
{
    public int WweiaFoodCategoryCode { get; set; }
    public string WweiaFoodCategoryDescription { get; set; } = string.Empty;
}
