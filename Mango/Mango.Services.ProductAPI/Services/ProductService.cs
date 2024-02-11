using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;

        public ProductService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public IEnumerable<ProductDto> GetProducts()
        {
            IEnumerable<Product> objList = _db.Products.ToList();
            IEnumerable<ProductDto> productDtos = _mapper.Map<IEnumerable<ProductDto>>(objList);
            return productDtos;
        }

        public ProductDto GetProduct(int id)
        {
            Product obj = _db.Products.First(u => u.ProductId == id);
            ProductDto productDto = _mapper.Map<ProductDto>(obj);
            return productDto;
        }

        public ProductDto SaveProduct(ProductDto ProductDto, string baseUrl)
        {
            Product product = _mapper.Map<Product>(ProductDto);
            product.SetImage(ProductDto, baseUrl);
            _db.Products.Add(product);
            _db.SaveChanges();
            ProductDto savedProductDto = _mapper.Map<ProductDto>(product);
            return savedProductDto;
        }

        public ProductDto UpdateProduct(ProductDto ProductDto, string baseUrl)
        {
            Product product = _mapper.Map<Product>(ProductDto);

            product.UpdateImage(ProductDto, baseUrl);

            _db.Products.Update(product);
            _db.SaveChanges();

            ProductDto updatedProductDto = _mapper.Map<ProductDto>(product);
            return updatedProductDto;
        }

        public void DeleteProduct(int id)
        {
            Product obj = _db.Products.First(u => u.ProductId == id);
            if (!string.IsNullOrEmpty(obj.ImageLocalPath))
            {
                var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
                FileInfo file = new FileInfo(oldFilePathDirectory);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            _db.Products.Remove(obj);
            _db.SaveChanges();
        }
    }
}
