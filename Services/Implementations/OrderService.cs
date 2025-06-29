using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THweb.Data;
using THweb.Models.Entities;
using THweb.Services.Interfaces;

namespace THweb.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public OrderService(ApplicationDbContext context, ICartService cartService, IProductService productService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public async Task<Order> CreateOrderAsync(string userId, string shippingAddress)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                return null;

            // 1. Kiểm tra tồn kho trước khi tạo đơn hàng
            foreach (var item in cart.CartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || item.Quantity > product.StockQuantity)
                {
                    throw new InvalidOperationException($"Số lượng đặt hàng ({item.Quantity}) vượt quá số lượng tồn kho ({product?.StockQuantity ?? 0}) cho sản phẩm ID {item.ProductId}.");
                }
            }

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                Status = OrderStatus.Pending
            };

            foreach (var item in cart.CartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _cartService.ClearCartAsync(userId);
            return order;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => userId == null || o.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(bool, string)> CreateOrderAsync(Order order, List<CartItem> cartItems)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Kiểm tra tồn kho cho tất cả sản phẩm trong giỏ hàng
                    foreach (var item in cartItems)
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product == null || item.Quantity > product.StockQuantity)
                        {
                            await transaction.RollbackAsync();
                            return (false, $"Số lượng đặt hàng ({item.Quantity}) vượt quá số lượng tồn kho ({product?.StockQuantity ?? 0}) cho sản phẩm ID {item.ProductId}.");
                        }
                    }

                    // 2. Nếu tất cả sản phẩm đều đủ hàng, tiến hành tạo đơn hàng
                    order.OrderDate = DateTime.Now;
                    order.Status = OrderStatus.Pending;
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // 3. Tạo chi tiết đơn hàng và trừ số lượng tồn kho
                    foreach (var item in cartItems)
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = (await _context.Products.FindAsync(item.ProductId)).Price
                        };
                        _context.OrderDetails.Add(orderDetail);

                        // Trừ số lượng tồn kho
                        var productToUpdate = await _context.Products.FindAsync(item.ProductId);
                        productToUpdate.StockQuantity -= item.Quantity;
                    }

                    await _context.SaveChangesAsync();

                    // Nếu mọi thứ thành công, commit transaction
                    await transaction.CommitAsync();

                    return (true, "Đặt hàng thành công!");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, "Đã xảy ra lỗi trong quá trình xử lý đơn hàng. Vui lòng thử lại.");
                }
            }
        }
    }
}