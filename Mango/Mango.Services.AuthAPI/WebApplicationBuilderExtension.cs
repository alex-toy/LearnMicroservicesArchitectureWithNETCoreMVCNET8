﻿using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mango.MessageBus;

namespace Mango.Services.AuthAPI
{
    public static class WebApplicationBuilderExtension
    {
        public static void ConfigureDbContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AuthDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
        }

        public static void ConfigureIdentity(this WebApplicationBuilder builder)
        {
            IConfigurationSection config = builder.Configuration.GetSection("ApiSettings:JwtOptions");
            builder.Services.Configure<JwtOptions>(config);
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IServiceBus, ServiceBus>();
        }
    }
}
