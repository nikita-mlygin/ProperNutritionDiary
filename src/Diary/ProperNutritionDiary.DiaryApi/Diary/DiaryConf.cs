using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.DiaryApi.Diary;

public class DiaryConf : IEntityTypeConfiguration<Diary>
{
    public void Configure(EntityTypeBuilder<Diary> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.DiaryEntries).WithOne().HasForeignKey("DiaryEntryId").IsRequired();

        builder.Property(x => x.Date).IsRequired();
    }
}
