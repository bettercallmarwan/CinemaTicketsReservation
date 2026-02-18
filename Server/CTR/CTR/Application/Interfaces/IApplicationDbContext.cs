using CTR.Models.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CTR.Application.Interfaces
{
    public interface IApplicationDbContext 
    {
        public DbSet<Movie> Movies { get; }
        public DbSet<Seat> Seats { get; }
        public DbSet<User> User { get; }
        public DbSet<Reservation> Reservations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DatabaseFacade Database { get; }
    }
}
