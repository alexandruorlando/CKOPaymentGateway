namespace PaymentGateway.Domain.Services;

public static class ErrorMessage
{
    public const string PaymentBankFailure = "Failed to process payment.";
    public const string PaymentTokenizerFailure = "Failed to tokenize card information.";
    public const string ApplicationFailure = "Error processing payment. Please try again later.";
}