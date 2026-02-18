using CTR.Application.DTOs.Reservation;
using CTR.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CTR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService bookingService)
        {
            _reservationService = bookingService;
        }

        [Authorize]
        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveSeat([FromBody]ReservationRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var result = await _reservationService.ReserveSeatAsync(dto.SeatId, userId);
            return this.GetResponse<int>(result);
        }

        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelReservation([FromBody]CancelReservationRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var result = await _reservationService.CancelReservationAsync(dto.ReservationId, userId);
            return this.GetResponse<CancelReservationResponseDto>(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetUserReservations()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var result = await _reservationService.GetUserReservationsAsync(userId);
            return this.GetResponse<IEnumerable<ReservationResponseDto>>(result);
        }
    }
}
