namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

//     SRLegacyFoodItem:
//       required:
//         - fdcId
//         - dataType
//         - description
//       properties:
//         fdcId:
//           type: integer
//           example: 170379
//         dataType:
//           type: string
//           example: "SR Legacy"
//         description:
//           type: string
//           example: "Broccoli, raw"
//         foodClass:
//           type: string
//           example: "FinalFood"
//         isHistoricalReference:
//           type: boolean
//           example: true
//         ndbNumber:
//           type: integer
//           example: 11090
//         publicationDate:
//           type: string
//           example: "4/1/2019"
//         scientificName:
//           type: string
//           example: "Brassica oleracea var. italica"
//         foodCategory:
//           $ref: '#/components/schemas/FoodCategory'
//         foodNutrients:
//           type: array
//           items:
//             $ref: '#/components/schemas/FoodNutrient'
//         nutrientConversionFactors:
//           type: array
//           items:
//             $ref: '#/components/schemas/NutrientConversionFactors'

public class SRLegacyFoodItem
{
    public int FdcId { get; set; }
    public string DataType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FoodClass { get; set; }
    public bool? IsHistoricalReference { get; set; }
    public int? NdbNumber { get; set; }
    public string? PublicationDate { get; set; }
    public string? ScientificName { get; set; }
    public FoodCategory? FoodCategory { get; set; }
    public FoodNutrient[]? FoodNutrients { get; set; }
    public NutrientConversionFactor[]? NutrientConversionFactors { get; set; }
}
