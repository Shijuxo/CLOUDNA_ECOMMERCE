namespace CLOUDNA_ECOMMERCE.Models
{
    public class OrderItem
    {

        public string OrderItemId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<Order> orders { get; set; }
        public List<Product> products { get; set; }
    }
}