using Mango.Services.ShoppingCartAPI.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureAuthentication();
builder.ConfigureMapper();
builder.ConfigureServices();
builder.ConfigureUris();
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

app.MapControllers();
app.ApplyMigration();
app.Run();