using THweb.Models.Entities;

namespace THweb.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, string shippingAddress);
        Task<(bool, string)> CreateOrderAsync(Order order, List<CartItem> cartItems);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
    }
}