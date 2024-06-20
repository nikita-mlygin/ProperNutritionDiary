using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProperNutritionDiary.DiaryContracts;

namespace ProperNutritionDiary.DiaryApi.Diary;

public class DiaryEntryConfiguration : IEntityTypeConfiguration<DiaryEntry>
{
    public void Configure(EntityTypeBuilder<DiaryEntry> builder)
    {
        builder.HasKey(de => de.Id);

        builder.Property(de => de.IdType).IsRequired();

        builder.Property(de => de.IdValue).IsRequired();

        builder.Property(de => de.ProductName).HasMaxLength(200).IsRequired();

        builder.Property(de => de.Weight).IsRequired();

        builder.Property(de => de.ConsumptionTime).IsRequired();
        builder
            .Property(de => de.ConsumptionType)
            .IsRequired()
            .HasDefaultValue(ConsumptionType.Other);

        builder.OwnsOne(
            de => de.Macronutrients,
            mac =>
            {
                mac.Property(m => m.Carbohydrates).IsRequired();
                mac.Property(m => m.Proteins).IsRequired();
                mac.Property(m => m.Fats).IsRequired();
                mac.Property(m => m.Calories).IsRequired();
            }
        );
    }
}
