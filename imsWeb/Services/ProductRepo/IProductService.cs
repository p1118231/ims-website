using System;
using imsWeb.Models;

namespace imsWeb.Services.ProductRepo;

public interface IProductService{

    Task<IEnumerable<Product>> GetProducts();

    
}