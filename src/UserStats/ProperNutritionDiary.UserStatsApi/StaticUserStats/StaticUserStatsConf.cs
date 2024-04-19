using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.UserStatsApi.StaticUserStats;

public class StaticUserStatsConf : IEntityTypeConfiguration<StaticUserStats>
{
    public void Configure(EntityTypeBuilder<StaticUserStats> builder)
    {
        builder.HasKey(ss => ss.Id);
        builder.HasIndex(ss => ss.UserId).IsUnique();
        builder.Property(ss => ss.Height).IsRequired(false);
        builder.Property(ss => ss.LifeStyle).IsRequired(false);
    }
}
