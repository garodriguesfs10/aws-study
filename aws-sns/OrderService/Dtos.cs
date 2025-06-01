namespace OrderService
{
    //just for test, i know that will be better in separate files
    public record ProductDetail(int ProductId, string Name, int Quantity);
    public record CreateOrderRequest(int OrderId, int CustomerId, List<ProductDetail> ProductDetails);
    public class OrderCreatedNotification
    {
        public int OrderId { get; init; }
        public int CustomerId { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<ProductDetail> ProductDetails { get; init; }

        public OrderCreatedNotification(int orderId, int customerId, List<ProductDetail> productDetails)
        {
            OrderId = orderId;
            CustomerId = customerId;
            CreatedAt = DateTime.UtcNow;
            ProductDetails = productDetails;
        }
    }
}
