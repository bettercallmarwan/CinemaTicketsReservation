using CTR.Models;

namespace CRS.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
