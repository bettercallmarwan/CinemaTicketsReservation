using CTR.Application.Extensions;

namespace CTR.Application.Interfaces
{
    public interface IReservationService
    {
        Task<Result<int>> ReserveSeatAsync(int seatId, int userId);
    }
}
