namespace ProperNutritionDiary.User.Api.Database;

using Microsoft.EntityFrameworkCore;
using ProperNutritionDiary.User.Api.User;
using ProperNutritionDiary.User.Api.User.Refresh;

public class AppCtx(DbContextOptions<AppCtx> configuration) : DbContext(configuration)
{
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<UserRefresh> Refreshes { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppCtx).Assembly);
    }
}
