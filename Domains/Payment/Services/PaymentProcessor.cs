using PaymentBroker.Domains.Payment.dtos;

namespace PaymentBroker.Domains.Payment.Services;

public interface IPaymentProcessor
{
	public Task<ProcessPaymentResponseDto> ProcessPayment(ProcessPaymentDto processPaymentDto);
	public Task<CheckProcessorHealthResponseDto> CheckProcessorHealth();
}