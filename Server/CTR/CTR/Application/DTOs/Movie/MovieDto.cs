using System.ComponentModel.DataAnnotations;
using CTR.Application.DTOs.Seats;
using CTR.Models.Classes;

namespace CTR.Application.DTOs.Movie
{
    public record MovieDto(int Id, string Title, string Hall, DateTime Date);

    public record CreateMovieDto(
        string Title,
        [property: RegularExpression(@"^[A-J]+$", ErrorMessage = "Hall can only contain capital letters from A to J (no spaces, digits, or other letters).")]
        string Hall,
        DateTime Date,
        List<CreateSeatDto> Seats
    );
    public record UpdateMovieDto(
        string Title,
        [property: RegularExpression(@"^[A-J]+$", ErrorMessage = "Hall can only contain capital letters from A to J (no spaces, digits, or other letters).")]
        string Hall,
        DateTime Date,
        List<CreateSeatDto> Seats);
    public record MovieWithSeatsDto(int Id, string Title, string Hall, DateTime Date, List<SeatDto> Seats);
}
