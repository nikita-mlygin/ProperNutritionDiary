using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProperNutritionDiary.User.Api.User.Refresh;

public class UserRefreshConf : IEntityTypeConfiguration<UserRefresh>
{
    public void Configure(EntityTypeBuilder<UserRefresh> builder)
    {
        builder.HasKey(c => new
        {
            c.UserId,
            c.Ip,
            c.DeviceHash
        });

        builder.HasOne<User>().WithMany().HasForeignKey(u => u.UserId).IsRequired();
        builder.Property(ur => ur.Ip).IsRequired();
        builder.Property(ur => ur.DateAdded).IsRequired();
        builder.Property(ur => ur.RT).IsRequired();
        builder.Property(ur => ur.DeviceHash).IsRequired();
    }
}
