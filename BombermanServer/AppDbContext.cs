using Microsoft.EntityFrameworkCore;

namespace BombermanServer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<PlayerStats> PlayerStats { get; set; } = null!;
        public DbSet<UserPreference> UserPreferences { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- Users ----
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                      .IsRequired();

                entity.HasIndex(u => u.Username)
                      .IsUnique();
            });

            // ---- PlayerStats ----
            modelBuilder.Entity<PlayerStats>(entity =>
            {
                entity.ToTable("PlayerStats");
                entity.HasKey(p => p.Id);

                // ⚠️ DefaultValue YOK — değerleri entity içinde veriyoruz
                entity.Property(p => p.Wins).IsRequired();
                entity.Property(p => p.Losses).IsRequired();

                entity.Property(p => p.LastPlayedAt);

                entity.HasOne(p => p.User)
                      .WithOne(u => u.Stats)
                      .HasForeignKey<PlayerStats>(p => p.UserId);
            });

            // ---- UserPreference ----
            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.ToTable("UserPreferences");
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.User)
                      .WithOne(u => u.Preferences)
                      .HasForeignKey<UserPreference>(p => p.UserId);
            });
        }
    }
}
