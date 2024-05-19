namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// FoodCategory:
//       properties:
//         id:
//           type: integer
//           format: int32
//           example: 11
//         code:
//           type: string
//           example: "1100"
//         description:
//           type: string
//           example: "Vegetables and Vegetable Products"

public class FoodCategory
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
