namespace PaymentGateway.Contracts.DTOs.v1;

public sealed record PaymentResult(
    bool Success,
    string Message,
    string? PaymentId
);