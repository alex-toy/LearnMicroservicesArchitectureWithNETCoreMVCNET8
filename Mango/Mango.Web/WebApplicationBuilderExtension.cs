using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Mango.Web
{
    public static class WebApplicationBuilderExtension
    {
        public static void ConfigureHttpServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<IProductService, ProductService>();
            builder.Services.AddHttpClient<ICouponService, CouponService>();
            builder.Services.AddHttpClient<ICartService, CartService>();
            builder.Services.AddHttpClient<IAuthService, AuthService>();
            builder.Services.AddHttpClient<IOrderService, OrderService>();
        }

        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
            SD.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"];
            SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
            SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
            SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];

            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICouponService, CouponService>();
        }

        public static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(10);
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                });
        }
    }
}
