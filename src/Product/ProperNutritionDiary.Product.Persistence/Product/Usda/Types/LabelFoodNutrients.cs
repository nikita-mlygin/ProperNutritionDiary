namespace ProperNutritionDiary.Product.Persistence.Product.Usda.Types;

public class LabelFoodNutrients
{
    public class NutrientValue
    {
        public decimal Value { get; set; }
    }

    public NutrientValue? Fat { get; set; }
    public NutrientValue? SaturatedFat { get; set; }
    public NutrientValue? TransFat { get; set; }
    public NutrientValue? Cholesterol { get; set; }
    public NutrientValue? Sodium { get; set; }
    public NutrientValue? Carbohydrates { get; set; }
    public NutrientValue? Fiber { get; set; }
    public NutrientValue? Sugars { get; set; }
    public NutrientValue? Protein { get; set; }
    public NutrientValue? Calcium { get; set; }
    public NutrientValue? Iron { get; set; }
    public NutrientValue? Potassium { get; set; }
    public NutrientValue? Calories { get; set; }
}
