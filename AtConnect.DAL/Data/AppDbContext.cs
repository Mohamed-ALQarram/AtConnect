using AtConnect.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AtConnect.DAL.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<IdentityUser> IdentityUsers { get; set; }
        public DbSet<AppUser> AppUsers{ get; set; }
        public DbSet<Chat> Chats{ get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatRequest> ChatRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DevcieToken> DevcieTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }

}
