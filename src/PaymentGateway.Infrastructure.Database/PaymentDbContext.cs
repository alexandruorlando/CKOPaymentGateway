using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Infrastructure.Database.Configuration;

namespace PaymentGateway.Infrastructure.Database;

public class PaymentDbContext : DbContext
{
    private readonly string _connectionString = null!;
    
    // Needed for EF Core
    public PaymentDbContext() {}
    
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options, IOptions<DatabaseConfiguration> dbConfig)
        : base(options)
    {
        _connectionString = dbConfig.Value.PaymentsConnectionString!;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
    }

    public DbSet<Payment> Payments { get; set; } = null!;
}