namespace CTR.Application.DTOs.Auth
{
    public record AuthResponseDto(string Token, string Email, string Name, string Role);
}
