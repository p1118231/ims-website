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

        public async Task<Product?> GetProductByIdAsync(int? id)
        {
            return await _context.Product.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return (IEnumerable<Product>)await _context.Product.ToListAsync();
        }
        
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        
    }
}
