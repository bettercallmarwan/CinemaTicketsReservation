using CTR.Application.DTOs.Seats;
using CTR.Models.Classes;

namespace CTR.Application.DTOs.Movie
{
    public record MovieDto(int Id, string Title, string Hall, DateTime Date);

    public record CreateMovieDto(string Title, string Hall, DateTime Date, List<CreateSeatDto> Seats);

    public record UpdateMovieDto(string Title, string Hall, DateTime Date, List<CreateSeatDto> Seats);
    public record MovieWithSeatsDto(int Id, string Title, string Hall, DateTime Date, List<SeatDto> Seats);
}
