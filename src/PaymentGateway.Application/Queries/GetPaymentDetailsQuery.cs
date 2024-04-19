using MediatR;
using PaymentGateway.Contracts.DTOs.v1;

namespace PaymentGateway.Application.Queries;

public record GetPaymentDetailsQuery(string PaymentId) : IRequest<PaymentDetails>;