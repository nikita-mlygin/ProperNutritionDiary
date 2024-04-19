using System.Data;
using Microsoft.EntityFrameworkCore;

namespace ProperNutritionDiary.UserMenuApi.Db;

using ProperNutritionDiary.UserMenuApi.UserMenu;

public class AppCtx(DbContextOptions<AppCtx> c) : DbContext(c)
{
    public DbSet<UserMenu> UserMenus { get; set; }
    public DbSet<UserMenuItem> UserMenuItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppCtx).Assembly);
    }
}
