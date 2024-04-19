namespace ProperNutritionDiary.UserPlanApi.UserPlan;

using ProperNutritionDiary.BuildingBlocks.ProductGlobals.Macronutrients;

public class UserPlan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public Macronutrients MacronutrientsGoal { get; set; } = default!;
}
