namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// InputFoodSurvey:
//       description: applies to Survey (FNDDS). Not all inputFoods will have all fields.
//       properties:
//         id:
//           type: integer
//           example: 18146
//         amount:
//           type: number
//           format: float
//           example: 1.5
//         foodDescription:
//           type: string
//           example: "Spices, curry powder"
//         ingredientCode:
//           type: integer
//           example: 2015
//         ingredientDescription:
//           type: string
//           example: "Spices, curry powder"
//         ingredientWeight:
//           type: number
//           format: float
//           example: 9.45
//         portionCode:
//           type: string
//           example: "21000"
//         portionDescription:
//           type: string
//           example: "1 tablespoon"
//         sequenceNumber:
//           type: integer
//           example: 6
//         surveyFlag:
//           type: integer
//           example: 0
//         unit:
//           type: string
//           example: "TB"
//         inputFood:
//           $ref: '#/components/schemas/SurveyFoodItem'
//         retentionFactor:
//           $ref: '#/components/schemas/RetentionFactor'

public class InputFoodSurvey
{
    public int Id { get; set; }
    public decimal? Amount { get; set; }
    public string FoodDescription { get; set; } = string.Empty;
    public int IngredientCode { get; set; }
    public string IngredientDescription { get; set; } = string.Empty;
    public decimal IngredientWeight { get; set; }
    public string? PortionCode { get; set; }
    public string? PortionDescription { get; set; }
    public int? SequenceNumber { get; set; }
    public int? SurveyFlag { get; set; }
    public string? Unit { get; set; }
    public SurveyFoodItem? InputFood { get; set; }
    public RetentionFactor? RetentionFactor { get; set; }
}
