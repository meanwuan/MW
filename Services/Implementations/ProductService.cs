using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using THweb.Data;
using THweb.Models.Entities;
using THweb.Services.Interfaces;

namespace THweb.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                                 .OrderByDescending(p => p.Id)
                                 .Take(4)
                                 .ToListAsync();
        }
        public async Task<(bool, string)> CheckProductStockAsync(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return (false, "Số lượng yêu cầu phải lớn hơn 0.");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return (false, $"Sản phẩm với ID {productId} không tồn tại.");
            }

            if (product.StockQuantity < quantity)
            {
                return (false, $"Sản phẩm '{product.Name}' không đủ số lượng tồn kho. Chỉ còn {product.StockQuantity} sản phẩm.");
            }

            return (true, $"Sản phẩm '{product.Name}' có đủ {quantity} sản phẩm trong kho.");
        }
    }
}