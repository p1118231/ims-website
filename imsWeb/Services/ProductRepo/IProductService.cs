using System;
using imsWeb.Models;

namespace imsWeb.Services.ProductRepo;

public interface IProductService{

    Task<IEnumerable<Product>> GetProducts();

    Task<Product?> GetProductByIdAsync(int? id);

    Task SaveChangesAsync();

    
}