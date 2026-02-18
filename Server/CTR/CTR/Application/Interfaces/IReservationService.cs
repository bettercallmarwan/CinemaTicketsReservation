using CTR.Application.DTOs.Reservation;
using CTR.Application.Extensions;

namespace CTR.Application.Interfaces
{
    public interface IReservationService
    {
        Task<Result<int>> ReserveSeatAsync(int seatId, int userId);
        Task<Result<CancelReservationResponseDto>> CancelReservationAsync(int reservationId, int userId);
        Task<Result<IEnumerable<ReservationResponseDto>>> GetUserReservationsAsync(int userId);
    }
}
