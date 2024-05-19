namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

//    MeasureUnit:
//       properties:
//         id:
//           type: integer
//           format: int32
//           example: 999
//         abbreviation:
//           type: string
//           example: "undetermined"
//         name:
//           type: string
//           example: "undetermined"

public class MeasureUnit
{
    public int Id { get; set; }
    public string Abbreviation { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
