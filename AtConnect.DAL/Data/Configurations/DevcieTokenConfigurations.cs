using AtConnect.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AtConnect.DAL.Data.Configurations
{
    public class DevcieTokenConfigurations : IEntityTypeConfiguration<DeviceToken>
    {
        public void Configure(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.HasKey(dt => dt.Id);

            builder.Property(dt => dt.Token)
                   .IsRequired()
                   .HasMaxLength(512);

            builder.Property(dt => dt.CreatedAt)
                   .IsRequired();

            builder.Property(dt => dt.IsActive)
                   .IsRequired();

            builder.HasOne(dt => dt.User)
                   .WithMany(u => u.DevcieTokens)
                   .HasForeignKey(dt => dt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(dt => dt.Token).IsUnique();
        }
    }
}
