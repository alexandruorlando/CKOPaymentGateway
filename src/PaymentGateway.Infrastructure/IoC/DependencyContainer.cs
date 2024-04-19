using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaymentGateway.Infrastructure.BankSimulator;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator;
using PaymentGateway.Infrastructure.Abstractions.Currency;
using PaymentGateway.Infrastructure.Abstractions.Data.Repositories;
using PaymentGateway.Infrastructure.Abstractions.Sequencers;
using PaymentGateway.Infrastructure.Abstractions.Time;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider;
using PaymentGateway.Infrastructure.Currency;
using PaymentGateway.Infrastructure.Data.Repositories;
using PaymentGateway.Infrastructure.Database;
using PaymentGateway.Infrastructure.Database.Configuration;
using PaymentGateway.Infrastructure.Time;
using PaymentGateway.Infrastructure.TokenServiceProvider;

namespace PaymentGateway.Infrastructure.IoC;

public static class DependencyContainer
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection serviceProvider)
    {
        serviceProvider.RegisterData();
        serviceProvider.RegisterBankSimulator();
        serviceProvider.RegisterTokenServiceProvider();
        serviceProvider.RegisterSequencer();
        serviceProvider.RegisterClock();
        serviceProvider.RegisterCurrency();
        return serviceProvider;
    }
    
    private static void RegisterData(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddDbContext<PaymentDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
            options.UseInMemoryDatabase(config.PaymentsDbName!);
        });
        serviceProvider.AddScoped<IPaymentRepository, PaymentRepository>();
    }

    private static void RegisterBankSimulator(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddScoped<IBankSimulatorService, BankSimulatorService>();
    }
    
    private static void RegisterTokenServiceProvider(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddScoped<ICardTokenizationService, CardTokenizationService>();
    }
    
    private static void RegisterSequencer(this IServiceCollection serviceProvider)
    {
        // Singleton to ensure only one instance handles consistent sequence of generated IDs
        serviceProvider.AddSingleton<IIdGenerator, TimestampIdGenerator>();
    }
    
    private static void RegisterClock(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddSingleton<IClock, SystemClock>();
    }
    
    private static void RegisterCurrency(this IServiceCollection serviceProvider)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var jsonFilePath = Path.Combine(basePath, "AllowedCurrencies.json");
        serviceProvider.AddSingleton<ICurrencyProvider>(new CurrencyProvider(jsonFilePath));
    }
}