# Payment Gateway API

## Introduction

The Payment Gateway API is a robust, secure, and easy-to-integrate solution for processing electronic payments. It offers merchants the capability to handle payments and retrieve details efficiently. It incorporates features such as idempotency, rate limiting, and health checks to ensure reliability and security.

## Features

- **Process Payments**: Submit payment requests with full card details.
- **Retrieve Payment Details**: Access historical payment information.
- **Idempotency**: Avoid duplicate charges on retries.
- **Rate Limiting**: Prevent abuse and ensure service quality.
- **Health Checks**: Continuous monitoring of system health.

## Requirements

- Docker & Docker Compose for containerisation.
- .NET 8 SDK & runtime.

## Quick Start

1. **Clone the repository**:
    ```bash
    git clone [REPO_URL]
    ```

2. **Build and deploy with Docker Compose**:
   Navigate to the project root directory and run:
    ```bash
    docker-compose up --build
    ```
   This spins up the Payment Gateway API at `http://localhost:8080/api/v1/payment`

3. **Accessing the API**: Use the provided curl commands or your preferred API client to interact with the service.

## Detailed Usage

### Process a Payment

Send a POST request to `/api/v1/payment` with the payment details and an `Idempotency-Key` header to guarantee transaction uniqueness.

**Bash**:
```bash
curl -X POST "http://localhost:8080/api/v1/payment" \
     -H "Content-Type: application/json" \
     -H "Idempotency-Key: $(uuidgen)" \
     -d '{
          "cardNumber":"4111111111111111",
          "cardExpiryMonth":12,
          "cardExpiryYear":2030,
          "cvv":123,
          "amount":150.00,
          "currency":"EUR"
         }'
```

**Cmd**:
```cmd
curl -X POST "http://localhost:8080/api/v1/payment" ^
     -H "Content-Type: application/json" ^
     -H "Idempotency-Key: YOUR_UNIQUE_IDEMPOTENCY_KEY" ^
     -d "{\"cardNumber\":\"4111111111111111\",\"cardExpiryMonth\":12,\"cardExpiryYear\":2030,\"cvv\":123,\"amount\":150.00,\"currency\":\"EUR\"}"
```

### Retrieve Payment Details

Issue a GET request to `/api/v1/payment/{id}` with the payment ID to obtain the transaction details.

**Bash/Cmd**:
```bash
curl -X GET "http://localhost:8080/api/v1/payment/YOUR_UNIQUE_PAYMENT_ID"
```

## Concurrency & Cancellation Support

The API is designed to handle concurrent requests gracefully, using a combination of idempotency checks and locking to ensure consistency without sacrificing performance. It also gracefully handles client request cancellations to free up resources.

## Security & Compliance

Sensitive card information is not stored. Instead, the `CardTokenizationService` is used to replace card details with tokens. This approach minimises the risk of sensitive data exposure and complies with PCI DSS requirements.

## Testing

Run the provided tests to validate functionality:

```bash
dotnet test
```

## Extending the API

The architecture supports scalability and further development. It leverages `MediatR` for CQRS, `Microsoft.AspNetCore.Mvc` for the RESTful interface, and custom middlewares for rate limiting and error handling.

## Configuration and Environmental Variables

Configure service parameters through environmental variables in the `docker-compose.yml` file.

## Build with Docker

Refer to the provided Dockerfile for instructions on building the image for the Payment Gateway API.

## Accepted Currencies

Currently supported currencies include GBP, USD, EUR, JPY, CAD, and AUD.

## Data Validation

The API validates all incoming data using annotations and custom validation logic to maintain data integrity.

## Monitoring and Logging

Utilise the health checks to monitor service health and logging to track operational data.

## Author

- Alex Orlando
