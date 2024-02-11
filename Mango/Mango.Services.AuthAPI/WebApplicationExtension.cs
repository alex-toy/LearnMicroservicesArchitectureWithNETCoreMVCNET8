using Mango.Services.AuthAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.AuthAPI
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
                var _db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
        }
    }
}
