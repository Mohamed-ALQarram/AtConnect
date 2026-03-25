using AtConnect.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AtConnect.DAL.Data.Configurations
{
    public class NotificationConfigurations : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.ReceiverId).IsRequired();

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(n => n.Type)
                   .IsRequired();

            builder.Property(n => n.CreatedAt)
                   .IsRequired();

            builder.Property(n => n.IsRead)
                   .IsRequired();

            builder.HasOne(n => n.Receiver)
                   .WithMany()
                   .HasForeignKey(n => n.ReceiverId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Chat)
                   .WithMany()
                   .HasForeignKey(n => n.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.ChatRequest)
                   .WithMany()
                   .HasForeignKey(n => n.ChatRequestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
