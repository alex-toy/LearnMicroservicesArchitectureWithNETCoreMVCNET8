using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            HttpClient client = _httpClientFactory.CreateClient("Product");
            HttpResponseMessage response = await client.GetAsync($"/api/product");
            string apiContet = await response.Content.ReadAsStringAsync();
            ResponseDto? responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContet);

            if (!responseDto.IsSuccess) return new List<ProductDto>();

            string? value = Convert.ToString(responseDto.Result);
            return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(value);
        }
    }
}
