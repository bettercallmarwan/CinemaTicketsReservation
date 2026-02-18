using CTR.Application.DTOs.Movie;
using CTR.Application.Extensions;

namespace CTR.Application.Interfaces
{
    public interface IMovieService
    {
        Task<Result<IEnumerable<MovieDto>>> GetAllAsync();
        Task<Result<MovieDto>> GetByIdAsync(int id);
        Task<Result<MovieDto>> CreateAsync(CreateMovieDto dto);
        Task<Result<MovieDto>> UpdateAsync(int id, UpdateMovieDto dto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<MovieWithSeatsDto>> GetSeatsAsync(int id);
    }
}
