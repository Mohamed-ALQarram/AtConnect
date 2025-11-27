using AtConnect.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace AtConnect.DAL.Configurations
{
    public class ChatRequestConfigurations : IEntityTypeConfiguration<ChatRequest>
    {
        public void Configure(EntityTypeBuilder<ChatRequest> builder)
        {
            builder.HasKey(cr => cr.Id);

            builder.Property(cr => cr.SenderId).IsRequired();
            builder.Property(cr => cr.ReceiverId).IsRequired();
            builder.Property(cr => cr.Status).IsRequired();
            builder.Property(cr => cr.CreatedAt).IsRequired();

            builder.HasOne(cr => cr.Sender)
                   .WithMany()
                   .HasForeignKey(cr => cr.SenderId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(cr => cr.Receiver)
                   .WithMany()
                   .HasForeignKey(cr => cr.ReceiverId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
