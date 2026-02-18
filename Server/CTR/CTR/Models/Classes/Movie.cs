namespace CTR.Models.Classes
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Hall { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<Seat> Seats { get; set; } = new();
    }
}
