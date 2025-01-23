using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using imsWeb.Models;

namespace imsWeb.Services.ProductRepo;

public interface IProductService{

    Task<IEnumerable<Product>> GetProducts();

    Task<Product?> GetProductByIdAsync(int? id);

    Task SaveChangesAsync();

    Task<bool> UpdateProduct(Product product);

    Task<bool> AddProduct(Product product);

    Task<bool> RemoveProduct(Product product);

    bool ProductExists(int id);

    
}