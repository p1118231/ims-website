using System;

namespace imsWeb.Services.ProductRepo;

public interface IProductService{

    Task<IEnumerable<ProductDto>> GetProducts();
}