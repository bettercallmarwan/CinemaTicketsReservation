using CTR.Models.Enums;

namespace CTR.Application.DTOs.Reservation
{
    public record ReservationDto(string Hall, string SeatNumber, string Movie, double Price, DateTime Date);
    public record ReservationRequestDto(int SeatId);
    public record ReservationResponseDto(string SeatNumber, string Movie, int? UserId, ReservationStatus Status, double Price, DateTime Date);
    public record CancelReservationRequestDto(int ReservationId);
    public record CancelReservationResponseDto(bool Cancelled, int ReservationId, string SeatNumber);


}
