namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// FoodAttribute:
//       properties:
//         id:
//           type: integer
//           example: 25117
//         sequenceNumber:
//           type: integer
//           example: 1
//         value:
//           type: string
//           example: "Moisture change: -5.0%"
//         FoodAttributeType:
//           type: object
//           properties:
//             id:
//               type: integer
//               example: 1002
//             name:
//               type: string
//               example: "Adjustments"
//             description:
//               type: string
//               example: "Adjustments made to foods, including moisture and fat changes."

public class FoodAttribute
{
    public int Id { get; set; }
    public int SequenceNumber { get; set; }
    public string Value { get; set; } = string.Empty;
}
