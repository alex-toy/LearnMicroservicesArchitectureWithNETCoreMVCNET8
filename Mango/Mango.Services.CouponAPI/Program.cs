using Mango.Services.CouponAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDbContext();
builder.ConfigureAutoMapper();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.ConfigureServices();

builder.ConfigureSwagger();
builder.ConfigureAuthentication();

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureSwagger();
//Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.ApplyMigration();
app.Run();