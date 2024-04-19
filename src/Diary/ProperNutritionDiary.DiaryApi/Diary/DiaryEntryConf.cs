using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.DiaryApi.Diary;

public class DiaryEntryConf : IEntityTypeConfiguration<DiaryEntry>
{
    public void Configure(EntityTypeBuilder<DiaryEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.Weight).IsRequired();
        builder.Property(x => x.ProductName).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.ConsumptionTime).IsRequired();
        builder.OwnsOne(x => x.Macronutrients);
    }
}
