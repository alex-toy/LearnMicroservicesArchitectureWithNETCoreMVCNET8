using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Extension
{
    public static class WebApplicationExtensions
    {
        public static void ConfigureDbContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
        }

        public static void ConfigureEmailService(this WebApplicationBuilder builder)
        {
            DbContextOptionsBuilder<AppDbContext> optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            builder.Services.AddSingleton(new EmailService(optionBuilder.Options));
        }
    }
}
