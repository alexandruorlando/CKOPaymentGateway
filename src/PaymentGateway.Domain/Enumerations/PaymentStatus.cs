namespace PaymentGateway.Domain.Enumerations;

public enum PaymentStatus
{
    NotProcessed,
    Processed,
    Failed,
    Cancelled,
    Pending,
    Declined,
    Refunded
}