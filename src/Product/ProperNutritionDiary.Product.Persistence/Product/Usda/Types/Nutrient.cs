namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

//    Nutrient:
//       description: a food nutrient
//       properties:
//         id:
//           type: integer
//           format: uint
//           example: 1005
//         number:
//           type: string
//           example: "305"
//         name:
//           type: string
//           example: "Carbohydrate, by difference"
//         rank:
//           type: integer
//           format: uint
//           example: 1110
//         unitName:
//           type: string
//           example: "g"
public class Nutrient
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Rank { get; set; }
    public string UnitName { get; set; } = string.Empty;
}
