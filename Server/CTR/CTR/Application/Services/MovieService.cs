using CTR.Application.DTOs;
using CTR.Application.Extensions;
using CTR.Application.Interfaces;
using CTR.Models;
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

            var movieDtos = movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title
            });

            return Result<IEnumerable<MovieDto>>.Ok(movieDtos);
        }

        public async Task<Result<MovieDto>> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return Result<MovieDto>.Fail("Movie not found.", HttpStatusCode.NotFound);

            return Result<MovieDto>.Ok(new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title
            });
        }

        public async Task<Result<MovieDto>> CreateAsync(CreateMovieDto dto)
        {
            var movie = new Movie
            {
                Title = dto.Title
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return Result<MovieDto>.Ok(new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title
            });
        }

        public async Task<Result<MovieDto>> UpdateAsync(int id, UpdateMovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return Result<MovieDto>.Fail("Movie not found.", HttpStatusCode.NotFound);

            movie.Title = dto.Title;
            await _context.SaveChangesAsync();

            return Result<MovieDto>.Ok(new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title
            });
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
