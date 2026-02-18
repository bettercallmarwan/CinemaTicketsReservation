using CTR.Models.Enums;

namespace CTR.Models.Classes;

public class Seat
{
    public int Id { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public double Price { get; set; }
    public int? MovieId { get; set; }
    public SeatStatus Status { get; set; } = SeatStatus.Free; // booked, locked, free
    
    public Movie? Movie { get; set; }
}