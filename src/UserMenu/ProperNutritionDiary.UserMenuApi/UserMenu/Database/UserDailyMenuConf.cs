namespace ProperNutritionDiary.UserMenuApi.UserMenu.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProperNutritionDiary.UserMenuApi.UserMenu.Entity;

public class UserDailyMenuConf : IEntityTypeConfiguration<UserDailyMenu>
{
    public void Configure(EntityTypeBuilder<UserDailyMenu> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.MenuItems).WithOne().HasForeignKey("UserMenuId");
        builder.Property(x => x.DayNumber).IsRequired();
    }
}
