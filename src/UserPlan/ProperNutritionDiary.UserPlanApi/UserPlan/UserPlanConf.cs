using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.UserPlanApi.UserPlan;

public class UserPlanConf : IEntityTypeConfiguration<UserPlan>
{
    public void Configure(EntityTypeBuilder<UserPlan> builder)
    {
        builder.HasKey(up => up.Id);
        builder.OwnsOne(up => up.MacronutrientsGoal);
        builder.Property(up => up.UserId).IsRequired();
        builder.Property(up => up.DateStart).IsRequired();
        builder.Property(up => up.DateEnd).IsRequired(false);
    }
}
