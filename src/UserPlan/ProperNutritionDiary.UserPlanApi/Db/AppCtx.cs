using Microsoft.EntityFrameworkCore;

namespace ProperNutritionDiary.UserPlanApi.Db;

using ProperNutritionDiary.UserPlanApi.UserPlan;

public class AppCtx(DbContextOptions<AppCtx> configuration) : DbContext(configuration)
{
    public DbSet<UserPlan> UserPlans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppCtx).Assembly);
    }
}
