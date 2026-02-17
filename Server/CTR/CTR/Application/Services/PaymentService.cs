using CTR.Application.Extensions;
using CTR.Application.Interfaces;
using CTR.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Net;

namespace CTR.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentService(IApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result<string>> CreateCheckoutSessionAsync(int reservationId, int userId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);

            if(reservation == null)
            {
                return Result<string>.Fail("Reservation cannot be found", HttpStatusCode.NotFound);
            }
            if (reservation.UserId != userId)
            {
                return Result<string>.Fail("Forbidden request", System.Net.HttpStatusCode.Forbidden);
            }
            if (reservation.Status != ReservationStatus.Pending)
            {
                return Result<string>.Fail("Reservation is no longer pending, try a new reservation", HttpStatusCode.BadRequest);
            }

            var options = CreateSessionOptions(reservation);

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            reservation.StripeSessionId = session.Id;
            await _context.SaveChangesAsync();

            return Result<string>.Ok(session.Url);
        }

        public async Task HandleCheckoutCompletedAsync(string json, string stripeSignature)
        {
            var webhookSecret = _configuration["Stripe:WebhookSecret"];

            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;

                if (session?.Metadata.TryGetValue("reservationId", out var reservationIdStr) == true
                    && int.TryParse(reservationIdStr, out var reservationId))
                {
                    var reservation = await _context.Reservations
                                .FromSql($"SELECT * FROM \"Reservations\" WHERE \"Id\" = {reservationId} FOR UPDATE")
                                .FirstOrDefaultAsync();

                    if (reservation != null && reservation.Status == ReservationStatus.Pending)
                    {
                        reservation.Status = ReservationStatus.Confirmed;

                        var seat = await _context.Seats.FirstOrDefaultAsync(s => s.SeatNumber == reservation.SeatNumber && s.MovieId == reservation.MovieId);

                        if (seat != null)
                        {
                            seat.Status = SeatStatus.Booked;
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        private SessionCreateOptions CreateSessionOptions(Reservation reservation)
        {
            return new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = _configuration["Stripe:SuccesUrl"],
                CancelUrl = _configuration["Stripe:CancelUrl"],
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = (long)(reservation.Price * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Cinema Ticket - {reservation.Hall} - Seat {reservation.SeatNumber}",
                                Description = $"Movie reservation on {reservation.Date:yyyy-MM-dd}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    { "reservationId", reservation.Id.ToString()}
                }
            };
        }

    }
}
