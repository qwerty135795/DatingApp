using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace API.Data;
public class DataContext : IdentityDbContext<AppUser,AppRole,
int,IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>,
IdentityRoleClaim<int>,IdentityUserToken<int>>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }
    public DbSet<UserLike> Likes {get;set;}
    public DbSet<Message> Messages { get;set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppUser>().HasMany(u => u.UserRoles).WithOne(j => j.User)
        .HasForeignKey(u => u.UserId).IsRequired();
        modelBuilder.Entity<AppRole>().HasMany(r => r.UserRoles).WithOne(j => j.Role)
        .HasForeignKey(r => r.RoleId).IsRequired();

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
