namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

//     FoodPortion:
//       properties:
//         id:
//           type: integer
//           format: int32
//           example: 135806
//         amount:
//           type: number
//           format: float
//           example: 1
//         dataPoints:
//           type: integer
//           format: int32
//           example: 9
//         gramWeight:
//           type: number
//           format: float
//           example: 91
//         minYearAcquired:
//           type: integer
//           example: 2011
//         modifier:
//           type: string
//           example: "10205"
//         portionDescription:
//           type: string
//           example: "1 cup"
//         sequenceNumber:
//           type: integer
//           example: 1
//         measureUnit:
//           $ref: '#/components/schemas/MeasureUnit'
public class FoodPortion
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int DataPoints { get; set; }
    public decimal GramWeight { get; set; }
    public int MinYearAcquired { get; set; }
    public string Modifier { get; set; } = string.Empty;
    public string PortionDescription { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public MeasureUnit MeasureUnit { get; set; } = default!;
}
