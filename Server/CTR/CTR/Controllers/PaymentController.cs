using CTR.Application.DTOs;
using CTR.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace CTR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var result = await _paymentService.CreateCheckoutSessionAsync(dto.ReservationId, userId);
            return this.GetResponse<string>(result);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                await _paymentService.HandleCheckoutCompletedAsync(json, stripeSignature!);
                return Ok();
            }
            catch (StripeException)
            {
                return BadRequest("Invalid Stripe signature");
            }
        }

        [HttpGet("checkout-success")]
        public IActionResult CheckoutSuccess([FromQuery] string session_id)
            => Ok(new { message = "Payment successful", sessionId = session_id });

        [HttpGet("checkout-cancelled")]
        public IActionResult CheckoutCancelled()
            => Ok(new { message = "Payment was cancelled" });
    }
}
