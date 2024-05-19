namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// SampleFoodItem:
//       required:
//         - fdcId
//         - dataType
//         - description
//       properties:
//         fdcId:
//           type: integer
//           example: 45551
//         datatype:
//           type: string
//           example: "Sample"
//         description:
//           type: string
//           example: "Beef, Tenderloin Roast, select, roasted, comp5, lean (34BLTR)"
//         foodClass:
//           type: string
//           example: "Composite"
//         publicationDate:
//           type: string
//           example: "4/1/2019"
//         foodAttributes:
//           type: array
//           items:
//             $ref: '#/components/schemas/FoodCategory'

public class SampleFoodItem
{
    public int FdcId { get; set; }
    public string DataType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FoodClass { get; set; }
    public string? PublicationDate { get; set; }
    public FoodCategory[]? FoodAttributes { get; set; }
}
