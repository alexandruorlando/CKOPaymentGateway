namespace PaymentGateway.Infrastructure.Abstractions.Sequencers;

public interface IIdGenerator
{
    long GenerateId();
}