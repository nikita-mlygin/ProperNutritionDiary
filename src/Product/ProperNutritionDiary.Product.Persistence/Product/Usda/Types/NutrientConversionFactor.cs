namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// NutrientConversionFactors:
//       properties:
//         type:
//           type: string
//           example: ".ProteinConversionFactor"
//         value:
//           type: number
//           format: float
//           example: 6.25000000

public class NutrientConversionFactor
{
    public string Type { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
