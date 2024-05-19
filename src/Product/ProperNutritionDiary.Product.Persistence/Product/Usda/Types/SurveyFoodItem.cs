namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

//     SurveyFoodItem:
//       required:
//         - fdcId
//         - dataType
//         - description
//       properties:
//         fdcId:
//           type: integer
//           example: 337985
//         datatype:
//           type: string
//           example: "Survey (FNDDS)"
//         description:
//           type: string
//           example: "Beef curry"
//         endDate:
//           type: string
//           example: "12/31/2014"
//         foodClass:
//           type: string
//           example: "Survey"
//         foodCode:
//           type: string
//           example: "27116100"
//         publicationDate:
//           type: string
//           example: "4/1/2019"
//         startDate:
//           type: string
//           example: "1/1/2013"
//         foodAttributes:
//           type: array
//           items:
//             $ref: '#/components/schemas/FoodAttribute'
//         foodPortions:
//           type: array
//           items:
//             $ref: '#/components/schemas/FoodPortion'
//         inputFoods:
//           type: array
//           items:
//             $ref: '#/components/schemas/InputFoodSurvey'
//         wweiaFoodCategory:
//           $ref: '#/components/schemas/WweiaFoodCategory'

public class SurveyFoodItem
{
    public int fdcId { get; set; }
    public string datatype { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public string? endDate { get; set; }
    public string? foodClass { get; set; }
    public string? foodCode { get; set; }
    public string? publicationDate { get; set; }
    public string? startDate { get; set; }
    public FoodAttribute[]? foodAttributes { get; set; }
    public FoodPortion[]? foodPortions { get; set; }
    public InputFoodSurvey[]? inputFoods { get; set; }
    public WweiaFoodCategory? wweiaFoodCategory { get; set; }
}
