using MemberManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace MemberManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Member> Members { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MemberNo).IsUnique();
            entity.HasIndex(e => e.Phone).IsUnique();
            entity.Property(e => e.MemberNo).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Remark).HasMaxLength(500);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
