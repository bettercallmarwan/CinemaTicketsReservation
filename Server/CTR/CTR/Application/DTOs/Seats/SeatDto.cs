using CTR.Models.Enums;

namespace CTR.Application.DTOs.Seats
{
    public record SeatDto(string SeatNumber, double Price, SeatStatus Status);
    public record CreateSeatDto(string SeatNumber, double Price);

}
