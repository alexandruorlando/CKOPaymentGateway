using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Commands;
using PaymentGateway.Application.Queries;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.Infrastructure.Abstractions.Idempotency;
using PaymentGateway.Infrastructure.Abstractions.Locking;

namespace PaymentGateway.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PaymentController(IMediator mediator, IIdempotencyService idempotencyService, ILockProvider lockProvider)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ProcessPayment(PaymentRequest request, [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            return BadRequest("Idempotency key is required.");
        }

        using var lockHandle = await lockProvider.AcquireLockAsync(idempotencyKey, cancellationToken);
        var cachedResult = await idempotencyService.GetCachedResponseAsync<PaymentResult>(idempotencyKey, cancellationToken);
        if (cachedResult is not null)
        {
            return Ok(cachedResult);
        }

        var response = await mediator.Send(new ProcessPaymentCommand(request.CardNumber, request.CardExpiryMonth, request.CardExpiryYear, request.CVV, request.Amount, request.Currency), cancellationToken);
        await idempotencyService.CacheResponseAsync(idempotencyKey, response, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentDetails(string id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPaymentDetailsQuery(id), cancellationToken);
        return Ok(result);
    }
}