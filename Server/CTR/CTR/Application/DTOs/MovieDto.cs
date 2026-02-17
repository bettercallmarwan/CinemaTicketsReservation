namespace CTR.Application.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    public class CreateMovieDto
    {
        public string Title { get; set; } = string.Empty;
    }

    public class UpdateMovieDto
    {
        public string Title { get; set; } = string.Empty;
    }
}
