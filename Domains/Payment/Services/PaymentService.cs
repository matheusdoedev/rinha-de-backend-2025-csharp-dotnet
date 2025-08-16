using PaymentBroker.Domains.Payment.dtos;

namespace PaymentBroker.Domains.Payment.Services;

public interface IPaymentService
{
	public Task<ReceivePaymentResponseDto> ReceivePayment(ReceivePaymentDto receivePaymentDto);
}