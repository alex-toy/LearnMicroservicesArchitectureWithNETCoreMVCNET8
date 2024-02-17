using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpRequestMessage message = new();
                SetMessageHeaders(message, requestDto);

                if (withBearer) HandleBearer(message);

                SetMessageContent(requestDto, message);

                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
                HttpResponseMessage? apiResponse = await GetApiResponse(requestDto, client, message);

                return await GetResponseDto(apiResponse);
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }

        private static void SetMessageHeaders(HttpRequestMessage message, RequestDto requestDto)
        {
            message.RequestUri = new Uri(requestDto.Url);

            if (requestDto.ContentType == ContentType.MultipartFormData)
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");
            }
        }

        private void HandleBearer(HttpRequestMessage message)
        {
            var token = _tokenProvider.GetToken();
            message.Headers.Add("Authorization", $"Bearer {token}");
        }

        private static void SetMessageContent(RequestDto requestDto, HttpRequestMessage message)
        {
            if (requestDto.ContentType == ContentType.MultipartFormData)
            {
                HandleMultipartFormData(requestDto, message);
            }
            else if (requestDto.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
            }
        }

        private static async Task<ResponseDto?> GetResponseDto(HttpResponseMessage? apiResponse)
        {
            return apiResponse.StatusCode switch
            {
                HttpStatusCode.NotFound => new ResponseDto() { IsSuccess = false, Message = "Not Found" },
                HttpStatusCode.Forbidden => new ResponseDto() { IsSuccess = false, Message = "Access Denied" },
                HttpStatusCode.Unauthorized => new ResponseDto() { IsSuccess = false, Message = "Unauthorized" },
                HttpStatusCode.InternalServerError => new ResponseDto() { IsSuccess = false, Message = "Internal Server Error" },
                _ => await GetOkResponseDto(apiResponse)
            };
        }

        private static async Task<ResponseDto?> GetOkResponseDto(HttpResponseMessage? apiResponse)
        {
            string apiContent = await apiResponse.Content.ReadAsStringAsync();
            ResponseDto? apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            return apiResponseDto;
        }

        private static async Task<HttpResponseMessage?> GetApiResponse(RequestDto requestDto, HttpClient client, HttpRequestMessage message)
        {
            message.Method = requestDto.ApiType switch
            {
                ApiType.POST => HttpMethod.Post,
                ApiType.DELETE => HttpMethod.Delete,
                ApiType.PUT => HttpMethod.Put,
                _ => HttpMethod.Get
            };

            HttpResponseMessage? apiResponse = await client.SendAsync(message);
            return apiResponse;
        }

        private static void HandleMultipartFormData(RequestDto requestDto, HttpRequestMessage message)
        {
            var content = new MultipartFormDataContent();

            foreach (var prop in requestDto.Data.GetType().GetProperties())
            {
                var value = prop.GetValue(requestDto.Data);
                if (value is FormFile)
                {
                    var file = (FormFile)value;
                    if (file != null)
                    {
                        content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                    }
                }
                else
                {
                    content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                }
            }
            message.Content = content;
        }
    }
}
