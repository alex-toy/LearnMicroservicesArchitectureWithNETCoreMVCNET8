using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI.Services
{
    public interface IProductService
    {
        void DeleteProduct(int id);
        ProductDto GetProduct(int id);
        IEnumerable<ProductDto> GetProducts();
        ProductDto SaveProduct(ProductDto ProductDto, string baseUrl);
        ProductDto UpdateProduct(ProductDto ProductDto, string baseUrl);
    }
}