using AutoMapper;
using CTR.Application.Extensions;
using CTR.Application.Interfaces;
using CTR.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Net;

namespace CTR.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReservationService(IApplicationDbContext context, IMapper mapping)
        {
            _context = context;
            _mapper = mapping;
        }

        public async Task<Result<int>> ReserveSeatAsync(int seatId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            try
            {
                var seat = await _context.Seats
                        .FromSql($"SELECT * FROM \"Seats\" WHERE \"Id\" = {seatId} FOR UPDATE NOWAIT")
                        .FirstOrDefaultAsync();

                if (seat == null)
                {
                    return Result<int>.Fail("Seat is not found", HttpStatusCode.NotFound);
                }

                if (seat.Status == SeatStatus.Booked || seat.Status == SeatStatus.Locked)
                {
                    return Result<int>.Fail("Seat is not currently available, try again later", HttpStatusCode.BadRequest);
                }

                var movie = await _context.Movies.FindAsync(seat.MovieId);

                var reservation = Reservation.Create(seat, movie!, userId);

                seat.Status = SeatStatus.Locked;

                await _context.Reservations.AddAsync(reservation);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Result<int>.Ok(reservation.Id);
            }
            catch (Exception ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "55P03") // cannot accquire lock
            {
                await transaction.RollbackAsync();
                return Result<int>.Fail("Seat is currently being reserved by someone else, try again in few minutes", HttpStatusCode.Conflict);
            }
            catch (Exception ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "40P01") // deadlock
            {
                await transaction.RollbackAsync();
                return Result<int>.Fail("Please try again", HttpStatusCode.Conflict);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
