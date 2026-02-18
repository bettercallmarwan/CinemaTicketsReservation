using AutoMapper;
using CTR.Application.DTOs.Reservation;
using CTR.Application.Extensions;
using CTR.Application.Interfaces;
using CTR.Models.Classes;
using CTR.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Stripe;
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

        public async Task<Result<CancelReservationResponseDto>> CancelReservationAsync(int reservationId, int userId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);

            if (reservation == null)
            {
                return Result<CancelReservationResponseDto>.Fail("Reservation cannot be found", HttpStatusCode.NotFound);
            }

            if (reservation.UserId != userId)
            {
                return Result<CancelReservationResponseDto>.Fail("Forbidden Request", HttpStatusCode.Forbidden);
            }

            if (reservation.Status != ReservationStatus.Confirmed)
            {
                return Result<CancelReservationResponseDto>.Fail("Reservation is not confirmed", HttpStatusCode.BadRequest);
            }

            reservation.Status = ReservationStatus.Cancelled;

            var seat = await _context.Seats.FirstOrDefaultAsync(s => s.SeatNumber == reservation.SeatNumber && s.MovieId == reservation.MovieId);

            if(seat == null)
            {
                return Result<CancelReservationResponseDto>.Fail("Seat cannot be found", HttpStatusCode.NotFound);
            }

            seat.Status = SeatStatus.Free;

            try
            {
                var sessionService = new Stripe.Checkout.SessionService();
                var session = await sessionService.GetAsync(reservation.StripeSessionId);

                var refundService = new RefundService();
                await refundService.CreateAsync(new RefundCreateOptions
                {
                    PaymentIntent = session.PaymentIntentId
                });
            }
            catch(StripeException ex)
            {
                return Result<CancelReservationResponseDto>.Fail($"Error Refunding : {ex.Message}", HttpStatusCode.InternalServerError);
            }

            await _context.SaveChangesAsync();

            var result = new CancelReservationResponseDto(true, reservationId, seat.SeatNumber);
            return Result<CancelReservationResponseDto>.Ok(result);
        }

        public async Task<Result<IEnumerable<ReservationResponseDto>>> GetUserReservationsAsync(int userId)
        {
            var reservations = await _context.Reservations.Where(r => r.UserId == userId).Include(r => r.Movie).ToListAsync();

            var reservationsDto = reservations.Select(r => new ReservationResponseDto(r.SeatNumber, r.Movie.Title, userId, r.Status, r.Price, r.Date)).ToList();

            return Result<IEnumerable<ReservationResponseDto>>.Ok(reservationsDto);
        }

    }
}
