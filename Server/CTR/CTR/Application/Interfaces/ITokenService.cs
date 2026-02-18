using CTR.Models.Classes;

namespace CRS.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
