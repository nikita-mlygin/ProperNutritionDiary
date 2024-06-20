using Microsoft.EntityFrameworkCore;

namespace ProperNutritionDiary.UserStatsApi.Db;

using ProperNutritionDiary.UserStatsApi.DynamicUserStats;
using ProperNutritionDiary.UserStatsApi.StaticUserStats;
using ProperNutritionDiary.UserStatsApi.UserTotalDailiesStats;

public class AppCtx(DbContextOptions<AppCtx> configuration) : DbContext(configuration)
{
    public DbSet<StaticUserStats> StaticUserStats { get; set; }
    public DbSet<DynamicUserStats> DynamicUserStats { get; set; }
    public DbSet<UserTotalDailiesStats> TotalDailiesStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppCtx).Assembly);
    }
}
