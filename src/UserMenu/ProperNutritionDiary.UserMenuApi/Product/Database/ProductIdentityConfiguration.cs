using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProperNutritionDiary.UserMenuApi.Product.Entity;

namespace ProperNutritionDiary.UserMenuApi.Product.Database;

public class ProductIdentityConfiguration : IEntityTypeConfiguration<ProductIdentity>
{
    public void Configure(EntityTypeBuilder<ProductIdentity> builder)
    {
        builder
            .HasDiscriminator<ProductIdentityType>("identityType")
            .HasValue<BarcodeProductIdentity>(ProductIdentityType.Barcode)
            .HasValue<SystemProductIdentity>(ProductIdentityType.SystemItem)
            .HasValue<EdamamProductIdentity>(ProductIdentityType.Edamam)
            .HasValue<UsdaProductIdentity>(ProductIdentityType.USDA);
    }
}
