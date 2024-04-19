using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.UserStatsApi.DynamicUserStats;

public class DynamicUserStatsConf : IEntityTypeConfiguration<DynamicUserStats>
{
    public void Configure(EntityTypeBuilder<DynamicUserStats> builder)
    {
        builder.HasKey(ds => ds.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Weight).IsRequired();
        builder.Property(x => x.StartDateTime).IsRequired();
        builder.Property(x => x.EndDateTime).IsRequired(false);
    }
}
