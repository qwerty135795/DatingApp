using API.Entities;
using Microsoft.EntityFrameworkCore;
namespace API.Data;
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }
    public DbSet<AppUser> Users {get;set;}
    public DbSet<UserLike> Likes {get;set;}
    public DbSet<Message> Messages { get;set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserLike>().HasKey(u => new { u.SourceUserId,u.TargetUserId});

        modelBuilder.Entity<UserLike>().HasOne(t => t.SourceUser)
        .WithMany(t => t.LikedUsers).HasForeignKey(t => t.SourceUserId).
        OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>().HasOne(t => t.TargetUser).
        WithMany(t => t.LikedByUser).HasForeignKey(t => t.TargetUserId)
        .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Message>().HasOne(m => m.Sender).
        WithMany(s => s.MessagesSent).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>().HasOne(m => m.Recipient)
        .WithMany(u => u.MessagesReceived)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
