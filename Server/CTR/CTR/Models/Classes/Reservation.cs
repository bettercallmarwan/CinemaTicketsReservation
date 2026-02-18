using CTR.Models.Enums;

namespace CTR.Models.Classes
{
    public class Reservation
    {
        #region Properties
        public int Id { get; set; }
        public string Hall { get; set; }
        public string SeatNumber { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
        public ReservationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public User User { get; set; }
        public Movie Movie { get; set; }


        public string? StripeSessionId { get; set; }

        #endregion

        #region Methods
        public static Reservation Create(Seat seat, Movie movie, int userId)
        {
            return new Reservation
            {
                Hall = movie.Hall,
                SeatNumber = seat.SeatNumber,
                UserId = userId,
                MovieId = movie.Id,
                Price = seat.Price,
                Date = movie.Date,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };
        } 
        #endregion
    }
}
