using Mango.Services.ShoppingCartAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Extensions
{
    public static class WebApplicationExtension
    {
        public static void ConfigureSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                if (!app.Environment.IsDevelopment())
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API");
                    c.RoutePrefix = string.Empty;
                }
            });
        }

        public static void ApplyMigration(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                AppDbContext _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
        }
    }
}
