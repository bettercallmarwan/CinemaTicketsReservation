using CTR.Application.Interfaces;
using CTR.Models.Classes;

namespace CRS.Infrastructure.Persistence
{
    public static class DbIntializer
    {
        public static async Task SeedAsync(IApplicationDbContext dbContext)
        {
            if (!dbContext.Movies.Any())
            {
                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "Chungking Express",
                        Seats = new List<Seat>
                        {
                            new Seat { SeatNumber = "A12", Price = 100 },
                            new Seat { SeatNumber = "A22", Price = 100 },
                            new Seat { SeatNumber = "A33", Price = 100 },
                            new Seat { SeatNumber = "A40", Price = 120 },
                            new Seat { SeatNumber = "A72", Price = 120 },
                            new Seat { SeatNumber = "A79", Price = 120 }
                        },
                        Hall = "A",
                        Date = DateTime.UtcNow
                    },
                    new Movie
                    {
                        Title = "Heat",
                        Seats = new List<Seat>
                        {
                            new Seat { SeatNumber = "B01", Price = 200 },
                            new Seat { SeatNumber = "B02", Price = 200 },
                            new Seat { SeatNumber = "B03", Price = 250 },
                            new Seat { SeatNumber = "B04", Price = 250 },
                            new Seat { SeatNumber = "B05", Price = 250 }
                        },
                        Hall = "B",
                        Date = DateTime.UtcNow
                    },
                    new Movie
                    {
                        Title = "A Clockwork Orange",
                        Seats = new List<Seat>
                        {
                            new Seat { SeatNumber = "C01", Price = 180 },
                            new Seat { SeatNumber = "C02", Price = 180 },
                            new Seat { SeatNumber = "C03", Price = 180 },
                            new Seat { SeatNumber = "C04", Price = 180 },
                            new Seat { SeatNumber = "C05", Price = 180 }
                        },
                        Hall = "C",
                        Date = DateTime.UtcNow
                    },
                    new Movie
                    {
                        Title = "Taxi Driver",
                        Seats = new List<Seat>
                        {
                            new Seat { SeatNumber = "D10", Price = 120 },
                            new Seat { SeatNumber = "D11", Price = 120 },
                            new Seat { SeatNumber = "D12", Price = 150 },
                            new Seat { SeatNumber = "D13", Price = 150 },
                            new Seat { SeatNumber = "D14", Price = 150 }
                        },
                        Hall = "D",
                        Date = DateTime.UtcNow
                    }
                };

                dbContext.Movies.AddRange(movies);
            }

            if(!dbContext.User.Where(u => u.Role == "Admin").Any())
            {
                var admin = new User
                {
                    Name = "Admin1",
                    Email = "Admin1@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1_"),
                    Role = "Admin"
                };

                await dbContext.User.AddAsync(admin);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
