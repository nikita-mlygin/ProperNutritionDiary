namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

// RetentionFactor:
//       properties:
//         id:
//           type: integer
//           example: 235
//         code:
//           type: integer
//           example: 3460
//         description:
//           type: string
//           example: "VEG, ROOTS, ETC, SAUTEED"

public class RetentionFactor
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Description { get; set; } = string.Empty;
}
