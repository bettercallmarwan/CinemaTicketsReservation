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

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                int pendingStatus = (int)ReservationStatus.Pending;
                int expiredStatus = (int)ReservationStatus.Expired;
                int freeSeatStatus = (int)SeatStatus.Free;
                DateTime now = DateTime.UtcNow;

                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    WITH expired_res AS (
                        SELECT ""Id"", ""SeatNumber"", ""MovieId""
                        FROM ""Reservations""
                        WHERE ""Status"" = {pendingStatus} AND ""ExpiresAt"" < {now}
                        FOR UPDATE SKIP LOCKED
                    ),
                    update_reservations AS (
                        UPDATE ""Reservations""
                        SET ""Status"" = {expiredStatus}
                        FROM expired_res
                        WHERE ""Reservations"".""Id"" = expired_res.""Id""
                    )
                    UPDATE ""Seats""
                    SET ""Status"" = {freeSeatStatus}
                    FROM expired_res
                    WHERE ""Seats"".""SeatNumber"" = expired_res.""SeatNumber"" 
                      AND ""Seats"".""MovieId"" = expired_res.""MovieId"";
                ", ct);

                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }
    }
}

