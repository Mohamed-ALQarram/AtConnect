using AtConnect.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AtConnect.DAL.Configurations
{
    public class MessageConfigurations : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.SenderId).IsRequired();
            builder.Property(m => m.ChatId).IsRequired();

            builder.Property(m => m.Content)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(m => m.SentAt)
                   .IsRequired();

            builder.Property(m => m.Status)
                   .IsRequired();

            builder.HasOne(m => m.Sender)
                   .WithMany()
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.Chat)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
