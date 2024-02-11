using Mango.Services.ProductAPI.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureDbContext();
builder.ConfigureAutoMapper();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.ConfigureSwagger();
builder.ConfigureAuthentication();
builder.Services.AddAuthorization();



WebApplication app = builder.Build();

app.ConfigureSwagger();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.ApplyMigration();
app.Run();

