using PaymentBroker.Domains.Payment.dtos;
using PaymentBroker.Providers;

namespace PaymentBroker.Domains.Payment.Services;

public class PaymentProcessorFallbackImpl : IPaymentProcessor
{
	private readonly string PROCESSOR_API_URL = Environment.GetEnvironmentVariable("PAYMENT_PROCESSOR_FALLBACK_URL") ?? throw new ArgumentException("fallback processor api url env not defined");

	public async Task<CheckProcessorHealthResponseDto> CheckProcessorHealth()
	{
		try
		{
			CheckProcessorHealthResponseDto checkProcessorHealthResponseDto = await HttpProvider.SendGetRequest<CheckProcessorHealthResponseDto>(PROCESSOR_API_URL + "/payments/service-health");

			return checkProcessorHealthResponseDto;
		}
		catch
		{
			throw;
		}
	}

	public async Task<ProcessPaymentResponseDto> ProcessPayment(ProcessPaymentDto processPaymentDto)
	{
		string url = PROCESSOR_API_URL + "/payments";
		ProcessPaymentResponseDto processPaymentResponseDto = await HttpProvider.SendPostRequest<ProcessPaymentDto, ProcessPaymentResponseDto>(url, processPaymentDto);

		return processPaymentResponseDto;
	}
}