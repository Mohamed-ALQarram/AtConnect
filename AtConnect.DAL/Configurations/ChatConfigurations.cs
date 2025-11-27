using AtConnect.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AtConnect.DAL.Configurations
{
    public class ChatConfigurations : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.User1Id).IsRequired();
            builder.Property(c => c.User2Id).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();

            builder.HasOne(c => c.User1)
              .WithMany()
              .HasForeignKey(c => c.User1Id)
              .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.User2)
                   .WithMany()
                   .HasForeignKey(c => c.User2Id)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Chat)
                   .HasForeignKey(m => m.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
