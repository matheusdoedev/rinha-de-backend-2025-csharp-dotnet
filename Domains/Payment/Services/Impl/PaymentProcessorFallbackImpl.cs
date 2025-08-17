using PaymentBroker.Domains.Payment.dtos;

namespace PaymentBroker.Domains.Payment.Services;

public class PaymentProcessorFallbackImpl : IPaymentProcessor
{
	public Task<CheckProcessorHealthResponseDto> CheckProcessorHealth()
	{
		throw new NotImplementedException();
	}

	public Task<ProcessPaymentResponseDto> ProcessPayment(ProcessPaymentDto processPaymentDto)
	{
		throw new NotImplementedException();
	}
}