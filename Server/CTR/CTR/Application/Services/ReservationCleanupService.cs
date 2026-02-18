using CTR.Application.Interfaces;
using CTR.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CTR.Application.Services
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReservationCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                    using var transaction = await _context.Database.BeginTransactionAsync();

                    var expiredId = await _context.Reservations
                        .Where(r => r.Status == ReservationStatus.Pending && r.ExpiresAt < DateTime.UtcNow)
                        .Select(r => r.Id)
                        .ToListAsync();

                    foreach(var id in expiredId)
                    {
                        var reservation = await _context.Reservations
                            .FromSql($"SELECT * FROM \"Reservations\" WHERE \"Id\" = {id} FOR UPDATE")
                            .FirstOrDefaultAsync();

                        if (reservation == null || reservation.Status != ReservationStatus.Pending)
                            continue; //already confirmed by webhook

                        reservation.Status = ReservationStatus.Expired;

                        var seat = await _context.Seats.FirstOrDefaultAsync(s => s.SeatNumber == reservation.SeatNumber && s.MovieId == reservation.MovieId);
                        if (seat != null) seat.Status = SeatStatus.Free;    
                    }

                    await _context.SaveChangesAsync(stoppingToken);
                    await transaction.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
