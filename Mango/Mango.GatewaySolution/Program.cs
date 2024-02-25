using Mango.GatewaySolution.Extensions;
using Ocelot.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureAuthentication();
builder.ConfigureOcelot();





WebApplication app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseOcelot().GetAwaiter().GetResult();
app.Run();
