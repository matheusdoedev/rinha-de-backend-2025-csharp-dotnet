using PaymentBroker.Domains.Payment.dtos;

namespace PaymentBroker.Domains.Payment.Services;

public interface IPaymentService
{
	public Task<ReceivePaymentResponseDto> ReceivePayment(ReceivePaymentDto receivePaymentDto);
	public Task ResendPaymentToWaitingQueue(SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto);
	public Task ResendPaymentToFallbackQueue(SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto);
}