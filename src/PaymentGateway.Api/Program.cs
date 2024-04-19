using PaymentGateway.Api.IoC;
using PaymentGateway.Api.Middlewares;
using PaymentGateway.Application.IoC;
using PaymentGateway.Caching.IoC;
using PaymentGateway.Domain.Services.IoC;
using PaymentGateway.Infrastructure.Configurations;
using PaymentGateway.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("Database"));

builder.Services
    .RegisterApi()
    .RegisterApplication()
    .RegisterDomainServices()
    .RegisterInfrastructure()
    .RegisterCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RateLimitingMiddleware>(100, 60);
app.MapHealthChecks("/api/status").AllowAnonymous();
app.MapControllers();

app.Run();

namespace PaymentGateway.Api
{
    public abstract partial class Program { }
}