namespace CLOUDNA_ECOMMERCE.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public string CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime DeliveryExpected { get; set; }
        public bool ContainsGift { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        
    }
}
