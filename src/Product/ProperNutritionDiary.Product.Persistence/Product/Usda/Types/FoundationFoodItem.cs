namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// FoundationFoodItem:
//       required:
//         - fdcId
//         - dataType
//         - description
//       properties:
//         fdcId:
//           type: integer
//           example: 747448
//         dataType:
//           type: string
//           example: "Foundation"
//         description:
//           type: string
//           example: "Strawberries, raw"
//         foodClass:
//           type: string
//           example: "FinalFood"
//         footNote:
//           type: string
//           example: "Source number reflects the actual number of samples analyzed for a nutrient. Repeat nutrient analyses may have been done on the same sample with the values shown."
//         isHistoricalReference:
//           type: boolean
//           example: false
//         ndbNumber:
//           type: integer
//           example: 9316
//         publicationDate:
//           type: string
//           example: "12/16/2019"
//         scientificName:
//           type: string
//           example: "Fragaria X ananassa"
//         foodCategory:
//           $ref: '#/components/schemas/FoodCategory'
//         foodComponents:
//           type: array
//           items:
//             $ref: '#/components/schemas/FoodComponent'
//         foodNutrients:
//           type: array
//           items:
//             $ref: '#/components/schemas/FoodNutrient'
//         foodPortions:
//           type: array
//           items:
//             $ref: '#/components/schemas/FoodPortion'
//         inputFoods:
//           type: array
//           items:
//             $ref: '#/components/schemas/InputFoodFoundation'
//         nutrientConversionFactors:
//           type: array
//           items:
//             $ref: '#/components/schemas/NutrientConversionFactors'
public class FoundationFoodItem
{
    public int FdcId { get; set; }
    public string DataType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FoodClass { get; set; }
    public string? FootNote { get; set; }
    public bool? IsHistoricalReference { get; set; }
    public int? NdbNumber { get; set; }
    public string? PublicationDate { get; set; }
    public string? ScientificName { get; set; }
    public FoodNutrient[]? FoodNutrients { get; set; }
    public FoodComponent[]? FoodComponents { get; set; }
    public InputFoodFoundation[]? InputFoods { get; set; }
    public FoodCategory? FoodCategory { get; set; }
    public FoodPortion[]? FoodPortions { get; set; }
    public NutrientConversionFactor[]? NutrientConversionFactors { get; set; }
}
