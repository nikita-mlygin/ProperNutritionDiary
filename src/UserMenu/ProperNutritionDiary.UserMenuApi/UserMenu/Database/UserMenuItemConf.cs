using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.UserMenuApi.UserMenu.Database;

using ProperNutritionDiary.UserMenuApi.UserMenu.Entity;

public class UserMenuItemConf : IEntityTypeConfiguration<UserMenuItem>
{
    public void Configure(EntityTypeBuilder<UserMenuItem> builder)
    {
        builder.HasKey(umi => umi.Id);
        builder
            .HasOne(umi => umi.ProductId)
            .WithMany()
            .HasForeignKey(umi => umi.ProductIdentityId)
            .IsRequired();
        builder.OwnsOne(umi => umi.Macronutrients);
    }
}
