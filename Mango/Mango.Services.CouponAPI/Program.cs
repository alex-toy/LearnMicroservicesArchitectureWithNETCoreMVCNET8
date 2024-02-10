using AutoMapper;
using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Extensions;
using Mango.Services.CouponAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDbContext();
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<ICouponService, CouponService>();

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