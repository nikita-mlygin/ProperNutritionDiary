namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// FoodNutrient:
//       required:
//         - id
//         - nutrientNumber
//         - unit
//       properties:
//         id:
//           type: integer
//           format: uint
//           example: 167514
//         amount:
//           type: number
//           format: float
//           example: 0E-8
//         dataPoints:
//           type: integer
//           format: int32
//           example: 49
//         min:
//           type: number
//           format: float
//           example: 73.73000000
//         max:
//           type: number
//           format: float
//           example: 91.80000000
//         median:
//           type: number
//           format: float
//           example: 90.30000000
//         type:
//           type: string
//           example: "FoodNutrient"
//         nutrient:
//           $ref: '#/components/schemas/Nutrient'
//         foodNutrientDerivation:
//           $ref: '#/components/schemas/FoodNutrientDerivation'
//         nutrientAnalysisDetails:
//           $ref: '#/components/schemas/NutrientAnalysisDetails'
public class FoodNutrient
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? Min { get; set; }
    public decimal? Max { get; set; }

    public Nutrient Nutrient { get; set; } = default!;
}
