namespace CTR.Application.DTOs
{
    public class ReserveTicketResponseDto
    {
        public string SeatNumber { get; set; } = string.Empty;
        public string Movie { get; set; }
        public int? UserId { get; set; }
        public bool Booked { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
    }
}
