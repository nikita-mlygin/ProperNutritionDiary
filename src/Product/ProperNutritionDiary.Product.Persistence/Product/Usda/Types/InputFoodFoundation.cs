namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

//     InputFoodFoundation:
//       description: applies to Foundation foods. Not all inputFoods will have all fields.
//       properties:
//         id:
//           type: integer
//           example: 45551
//         foodDescription:
//           type: string
//           example: Beef, Tenderloin Roast, select, roasted, comp5, lean (34BLTR)
//         inputFood:
//           $ref: '#/components/schemas/SampleFoodItem'

public class InputFoodFoundation
{
    public int Id { get; set; }
    public string FoodDescription { get; set; } = string.Empty;
    public SampleFoodItem InputFood { get; set; } = default!;
}
