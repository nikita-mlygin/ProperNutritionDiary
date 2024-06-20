using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.UserStatsApi.UserTotalDailiesStats;

public class UserTotalDailiesStatsConfiguration : IEntityTypeConfiguration<UserTotalDailiesStats>
{
    public void Configure(EntityTypeBuilder<UserTotalDailiesStats> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.Day).IsRequired();
        builder.OwnsOne(
            e => e.TotalMacronutrients,
            macronutrients =>
            {
                macronutrients.Property(m => m.Carbohydrates).HasColumnName("Carbohydrates");
                macronutrients.Property(m => m.Proteins).HasColumnName("Proteins");
                macronutrients.Property(m => m.Fats).HasColumnName("Fats");
                macronutrients.Property(m => m.Calories).HasColumnName("Calories");
            }
        );
    }
}
