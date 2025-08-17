using PaymentBroker.Domains.Payment.dtos;
using PaymentBroker.Domains.Payment.Repositories;
using PaymentBroker.Providers;

namespace PaymentBroker.Domains.Payment.Services;

public class PaymentService(IPaymentRepository paymentRepository) : IPaymentService
{
	private readonly IPaymentRepository _paymentRepository = paymentRepository;

	public async Task<ReceivePaymentResponseDto> ReceivePayment(ReceivePaymentDto receivePaymentDto)
	{
		string paymentId = await SavePayment(receivePaymentDto);

		await SendPaymentToWaitingQueue(paymentId);
		return GetResponse();
	}

	private async Task<string> SavePayment(ReceivePaymentDto receivePaymentDto)
	{
		Payment payment = new()
		{
			CorrelationId = receivePaymentDto.CorrelationId,
			Amount = receivePaymentDto.Amount,
		};

		await _paymentRepository.Add(payment);
		await _paymentRepository.Save();
		return payment.Id;
	}

	private async Task SendPaymentToWaitingQueue(string PaymentId)
	{
		SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto = new()
		{
			PaymentId = PaymentId
		};

		await BrokerProvider.SendMessage("waiting", sendPaymentToWaitingQueueDto);
	}

	private ReceivePaymentResponseDto GetResponse()
	{
		ReceivePaymentResponseDto receivePaymentResponseDto = new()
		{
			Message = "The payment is processing."
		};

		return receivePaymentResponseDto;
	}

	public async Task ResendPaymentToWaitingQueue(SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto)
	{
		await BrokerProvider.SendMessage("waiting", sendPaymentToWaitingQueueDto);
	}

	public async Task ResendPaymentToFallbackQueue(SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto)
	{
		await BrokerProvider.SendMessage("fallback", sendPaymentToWaitingQueueDto);
	}
}