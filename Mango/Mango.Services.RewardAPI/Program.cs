using Mango.Services.RewardAPI.Extension;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureDbContext();
builder.ConfigureServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



WebApplication app = builder.Build();
app.ConfigureSwagger();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.ApplyMigration();
app.ConfigureServiceBus();
app.Run();