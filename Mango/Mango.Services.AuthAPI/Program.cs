using Mango.Services.AuthAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureDbContext();
builder.ConfigureIdentity();

builder.Services.AddControllers();

builder.ConfigureServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

app.ConfigureSwagger();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.ApplyMigration();
app.Run();