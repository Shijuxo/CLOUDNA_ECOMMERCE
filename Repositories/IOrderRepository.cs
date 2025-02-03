using CLOUDNA_ECOMMERCE.Models;

namespace CLOUDNA_ECOMMERCE.Repositories
{
    // Interface that defines the contract for the OrderRepository
    public interface IOrderRepository
    {
        // Method to fetch the most recent order for a customer based on their email and customer ID
        Task<OrderResponse> GetMostRecentOrder(string userEmail, string customerId);
    }
}
