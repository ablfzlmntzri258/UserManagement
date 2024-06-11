using Microsoft.EntityFrameworkCore;
using UserManagement.Shared.Models;

namespace UserManagement.Repository.Contexts;

public class UserContext : DbContext
{
    private readonly string _connectionString;

    public UserContext()
    {
    }
    public UserContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    private readonly DbContextOptions<UserContext> _options;
    public UserContext(DbContextOptions<UserContext> contextOptions) : base(contextOptions)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=.;Database=UserManagement;Persist Security Info=False;User ID=ablfzlmntzri_121;Password=m47557482a;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;");
        base.OnConfiguring(optionsBuilder);
    }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(b => b.CreateAt)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
    }
}