namespace ProperNutritionDiary.UserMenuApi.UserMenu.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProperNutritionDiary.UserMenuApi.UserMenu.Entity;

public class UserMenuConf : IEntityTypeConfiguration<UserMenu>
{
    public void Configure(EntityTypeBuilder<UserMenu> builder)
    {
        builder.HasKey(um => um.Id);
        builder.HasMany(um => um.DailyMenus).WithOne().HasForeignKey("MenuId");

        builder.Property(um => um.UserId).IsRequired();
        builder.Property(um => um.Date).IsRequired();
    }
}
