using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.User.Api.User;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Login).IsUnique();

        builder.Property(x => x.Login).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
    }
}
