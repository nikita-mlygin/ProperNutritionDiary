namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

//     FoodComponent:
//       properties:
//         id:
//           type: integer
//           format: int32
//           example: 59929
//         name:
//           type: string
//           example: "External fat"
//         dataPoints:
//           type: integer
//           example: 24
//         gramWeight:
//           type: number
//           example: 2.1
//         isRefuse:
//           type: boolean
//           example: true
//         minYearAcquired:
//           type: integer
//           example: 2011
//         percentWeight:
//           type: number
//           example: 0.5

public class FoodComponent
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DataPoints { get; set; }
    public int GramWeight { get; set; }
    public bool IsRefuse { get; set; }
    public int MinYearAcquired { get; set; }
    public float PercentWeight { get; set; }
}
