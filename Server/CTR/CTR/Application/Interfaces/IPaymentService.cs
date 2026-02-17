using CTR.Application.Extensions;

namespace CTR.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<string>> CreateCheckoutSessionAsync(int reservationId, int userId);
        Task HandleCheckoutCompletedAsync(string json, string stripeSignature);
    }
}
