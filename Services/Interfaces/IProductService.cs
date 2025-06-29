using System.Collections.Generic;
using System.Threading.Tasks;
using THweb.Models.Entities;

namespace THweb.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        Task<(bool, string)> CheckProductStockAsync(int productId, int quantity);
    }
}