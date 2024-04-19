using AutoFixture;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.TestUtilities.Builders.Customisations;

namespace PaymentGateway.TestUtilities.Builders;

public class PaymentRequestBuilder
{
    private string _cardNumber = "4111111111111111"; // Default card number
        private int _cardExpiryMonth = 1;               // Default month
        private int _cardExpiryYear = DateTime.UtcNow.Year + 1; // Default year
        private int _cvv = 123;                         // Default CVV
        private decimal _amount = 1.0m;                 // Default amount
        private string _currency = "USD";  
    
    private readonly IFixture _fixture = new Fixture()
        .Customize(new ValidPaymentCommandCustomisation());

    public PaymentRequest BuildValid()
    {
        return _fixture.Create<PaymentRequest>();
    }

    public PaymentRequestBuilder WithCardNumber(string cardNumber)
        {
            _cardNumber = cardNumber;
            return this;
        }
    
        public PaymentRequestBuilder WithCardExpiryMonth(int month)
        {
            _cardExpiryMonth = month;
            return this;
        }
    
        public PaymentRequestBuilder WithCardExpiryYear(int year)
        {
            _cardExpiryYear = year;
            return this;
        }
    
        public PaymentRequestBuilder WithCVV(int cvv)
        {
            _cvv = cvv;
            return this;
        }
    
        public PaymentRequestBuilder WithAmount(decimal amount)
        {
            _amount = amount;
            return this;
        }
    
        public PaymentRequestBuilder WithCurrency(string currency)
        {
            _currency = currency;
            return this;
        }
    
        public PaymentRequest Build()
        {
            return new PaymentRequest(
                _cardNumber,
                _cardExpiryMonth,
                _cardExpiryYear,
                _cvv,
                _amount,
                _currency);
        }
}
