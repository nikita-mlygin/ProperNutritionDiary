using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.UserMenuApi.UserMenu;

public class UserMenuConf : IEntityTypeConfiguration<UserMenu>
{
    public void Configure(EntityTypeBuilder<UserMenu> builder)
    {
        builder.HasKey(um => um.Id);
        builder.HasMany(um => um.MenuItems).WithOne().HasForeignKey("MenuId").IsRequired();
        builder.Property(um => um.UserId).IsRequired();
        builder.Property(um => um.Date).IsRequired();
    }
}
