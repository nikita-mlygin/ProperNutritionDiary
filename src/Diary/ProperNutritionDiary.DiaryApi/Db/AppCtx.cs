using Microsoft.EntityFrameworkCore;

namespace ProperNutritionDiary.DiaryApi.Db;

using ProperNutritionDiary.DiaryApi.Diary;

public class AppCtx(DbContextOptions<AppCtx> configuration) : DbContext(configuration)
{
    public DbSet<Diary> Diaries { get; set; }
    public DbSet<DiaryEntry> DiaryEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppCtx).Assembly);
    }
}
