using CTR.Application.Interfaces;
using CTR.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace CTR.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<User> User => Set<User>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Seats)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seat>()
                .HasIndex(s => s.SeatNumber)
                .IsUnique();

        }
    }
}
