using AtConnect.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AtConnect.Core.Data.Configurations
{
    public class AppUserConfigurations : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasBaseType<IdentityUser>();
            builder.ToTable("Users");
            builder.Property(u => u.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.LastSeen)
                   .IsRequired();

            builder.Property(u => u.IsActive)
                   .IsRequired();

            builder.HasMany(u => u.DeviceTokens)
                   .WithOne(dt => dt.User)
                   .HasForeignKey(dt => dt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
