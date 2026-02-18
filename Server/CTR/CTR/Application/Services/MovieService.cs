using CTR.Application.DTOs.Movie;
using CTR.Application.DTOs.Seats;
using CTR.Application.Extensions;
using CTR.Application.Interfaces;
using CTR.Models;
using CTR.Models.Classes;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CTR.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IApplicationDbContext _context;

        public MovieService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<MovieDto>>> GetAllAsync()
        {
            var movies = await _context.Movies.ToListAsync();

            var movieDtos = movies.Select(m => new MovieDto(m.Id, m.Title, m.Hall, m.Date));

            return Result<IEnumerable<MovieDto>>.Ok(movieDtos);
        }

        public async Task<Result<MovieDto>> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return Result<MovieDto>.Fail("Movie not found.", HttpStatusCode.NotFound);

            return Result<MovieDto>.Ok(new MovieDto(movie.Id, movie.Title, movie.Hall, movie.Date));
        }

        public async Task<Result<MovieWithSeatsDto>> GetSeatsAsync(int id)
        {
            var movie = await _context.Movies.Include(m => m.Seats).FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return Result<MovieWithSeatsDto>.Fail("Movie not found.", HttpStatusCode.NotFound);

            var seatsDto = movie.Seats.Select(s => new SeatDto(s.SeatNumber, s.Price, s.Status)).ToList();

            return Result<MovieWithSeatsDto>.Ok(new MovieWithSeatsDto(movie.Id, movie.Title, movie.Hall, movie.Date, seatsDto));
        }

        public async Task<Result<MovieDto>> CreateAsync(CreateMovieDto dto)
        {
            var movie = new Movie
            {
                Title = dto.Title,
                Hall = dto.Hall,
                Date = dto.Date,
            };

            _context.Movies.Add(movie);

            var seats = dto.Seats.Select(s => new Seat
            {
                SeatNumber = s.SeatNumber,
                Price = s.Price
            }).ToList();

            movie.Seats = seats;

            await _context.SaveChangesAsync();

            return Result<MovieDto>.Ok(new MovieDto(movie.Id, movie.Title, movie.Hall, movie.Date));
        }

        public async Task<Result<MovieDto>> UpdateAsync(int id, UpdateMovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return Result<MovieDto>.Fail("Movie not found.", HttpStatusCode.NotFound);

            movie.Title = dto.Title;
            movie.Hall = dto.Hall;
            movie.Date = dto.Date;

            var seats = dto.Seats.Select(s => new Seat
            {
                SeatNumber = s.SeatNumber,
                Price = s.Price
            }).ToList();

            movie.Seats = seats;
            await _context.SaveChangesAsync();

            return Result<MovieDto>.Ok(new MovieDto(movie.Id, movie.Title, movie.Hall, movie.Date));
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return Result<bool>.Fail("Movie not found.", HttpStatusCode.NotFound);

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }
    }
}
