using AtConnect.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AtConnect.DAL.Data.Configurations
{
    public class IdentityUserConfigurations : IEntityTypeConfiguration<IdentityUser>
    {
        public void Configure(EntityTypeBuilder<IdentityUser> builder)
        {
            builder.HasKey(u => u.Id);
            builder.ToTable("Users");
            builder.Property(u => u.UserName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(254);

            builder.Property(u => u.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(u => u.RefreshToken)
                   .HasMaxLength(500);

            builder.Property(u => u.VerifyToken)
                   .HasMaxLength(500);

            builder.Property(u => u.RefreshTokenExpiryTime)
                   .IsRequired(false);

            builder.Property(u => u.VerifyTokenExpires)
                   .IsRequired(false);

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.UserName).IsUnique();
        }
    }
}
