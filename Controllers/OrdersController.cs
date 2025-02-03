using CLOUDNA_ECOMMERCE.Models;
using CLOUDNA_ECOMMERCE.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CLOUDNA_ECOMMERCE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        // Constructor to inject the IOrderRepository
        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // API endpoint to get the most recent order of a customer
        [HttpPost("recent")]
        public async Task<IActionResult> GetMostRecentOrder([FromBody] UserRequest request)
        {
            try
            {
                // Call the repository method to get the most recent order
                var response = await _orderRepository.GetMostRecentOrder(request.User, request.CustomerId);

                // If no response is found (i.e., invalid customer or email)
                if (response == null)
                    return BadRequest(new { message = "Invalid customer ID or email." });

                // Return the response with a 200 OK status
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Step 1: Log the exception (can be improved by using a logging library)
                Console.WriteLine($"Error: {ex.Message}");

                // Step 2: Return a 500 internal server error with a generic message
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request. Please try again later." });
            }
        }
    }
}
