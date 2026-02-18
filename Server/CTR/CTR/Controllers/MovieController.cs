using CTR.Application.DTOs.Movie;
using CTR.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CTR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _movieService.GetAllAsync();
            return this.GetResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _movieService.GetByIdAsync(id);
            return this.GetResponse(result);
        }

        [HttpGet("seats/{id}")]
        public async Task<IActionResult> GetSeats(int id)
        {
            var result = await _movieService.GetSeatsAsync(id);
            return this.GetResponse(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMovieDto dto)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            var result = await _movieService.CreateAsync(dto);
            return this.GetResponse(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMovieDto dto)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            var result = await _movieService.UpdateAsync(id, dto);
            return this.GetResponse(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            var result = await _movieService.DeleteAsync(id);
            return this.GetResponse(result);
        }
    }
}
