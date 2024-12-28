using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using imsWeb.Models; // Ensure the correct namespace for ProductDto

namespace imsWeb.Services.ProductRepo
{
    public class ProductService : IProductService
    {
        public Task<IEnumerable<ProductDto>> GetProducts()
        {
            throw new NotImplementedException();
        }

        
    }
}
