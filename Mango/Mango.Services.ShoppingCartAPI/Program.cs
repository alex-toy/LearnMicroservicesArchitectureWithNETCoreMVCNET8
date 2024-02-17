using Mango.Services.ShoppingCartAPI.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureDbContext();
builder.ConfigureAutoMapper();
builder.ConfigureServices();
builder.ConfigureUris();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.ConfigureSwagger();

builder.Services.AddAuthorization();




WebApplication app = builder.Build();
app.ConfigureSwagger();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.ApplyMigration();
app.Run();