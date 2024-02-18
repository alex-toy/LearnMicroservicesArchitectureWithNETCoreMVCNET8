using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Messaging;
using Mango.Services.RewardAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardAPI.Extension
{
    public static class WebApplicationBuilderExtension
    {
        public static void ConfigureDbContext(this WebApplicationBuilder builder)
        {
            string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(connectionString);
            });

            DbContextOptionsBuilder<AppDbContext> optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionBuilder.UseSqlServer(connectionString);
            builder.Services.AddSingleton(new RewardService(optionBuilder.Options));
        }

        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
        }
    }
}
