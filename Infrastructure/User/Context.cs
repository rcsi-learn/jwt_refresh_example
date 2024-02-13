using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.User
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Domain.User> Users { get; set; }
        public DbSet<Domain.RefreshTokenHistory> RefreshTokenHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.User>().ToTable("User");
            modelBuilder.Entity<Domain.RefreshTokenHistory>().ToTable("RefreshTokenHistory");

            modelBuilder.Entity<Domain.RefreshTokenHistory>()
            .HasOne(rth => rth.User)
            .WithMany()
            .HasForeignKey(rth => rth.IdUser)
            .IsRequired(false);

            // modelBuilder.Entity<Domain.RefreshTokenHistory>()
            //     .Property(rth => rth.Active);
                //.HasComputedColumnSql("CASE WHEN ExpirationDate <= DATETIME('now') THEN 0 ELSE 1 END");
        }
    }
}
