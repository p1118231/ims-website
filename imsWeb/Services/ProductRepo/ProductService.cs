using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using imsWeb.Models;
using imsWeb.Data;
using Microsoft.EntityFrameworkCore; // Ensure the correct namespace for ProductDto

namespace imsWeb.Services.ProductRepo
{


    public class ProductService : IProductService
    {

        private readonly ProductContext _context;

            public ProductService(ProductContext context)
        {
            _context = context;
        }

        public async Task<bool> AddProduct(Product product)
        {
             // Save changes
        await _context.Product.AddAsync(product);
        await _context.SaveChangesAsync();
        return true;

        }

        public async Task<Product?> GetProductByIdAsync(int? id)
        {
            return await _context.Product.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return (IEnumerable<Product>)await _context.Product.ToListAsync();
        }

        public bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        public async Task<bool> RemoveProduct(Product product)
        {
            _context.Product.Remove(product);
        await _context.SaveChangesAsync();
        return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            // Save changes
         _context.Product.Update(product);
        await _context.SaveChangesAsync();
        return true;
        }
    }
}
