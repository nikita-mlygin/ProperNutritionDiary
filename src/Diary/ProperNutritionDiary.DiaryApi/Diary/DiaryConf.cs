using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.DiaryApi.Diary;

public class DiaryConf : IEntityTypeConfiguration<Diary>
{
    public void Configure(EntityTypeBuilder<Diary> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.UserId).IsRequired();
        builder.Property(d => d.Date).IsRequired();

        builder
            .HasMany(d => d.DiaryEntries)
            .WithOne(e => e.Diary)
            .HasForeignKey(e => e.DiaryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
