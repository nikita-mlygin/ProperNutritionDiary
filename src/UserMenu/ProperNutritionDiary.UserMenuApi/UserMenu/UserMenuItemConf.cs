using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.UserMenuApi.UserMenu;

public class UserMenuItemConf : IEntityTypeConfiguration<UserMenuItem>
{
    public void Configure(EntityTypeBuilder<UserMenuItem> builder)
    {
        builder.HasKey(umi => umi.Id);
        builder.OwnsOne(umi => umi.Macronutrients);
        builder.Property(umi => umi.ConsumptionTime).IsRequired();
        builder.Property(umi => umi.ProductId).IsRequired();
        builder.Property(umi => umi.Weight).IsRequired();
    }
}
