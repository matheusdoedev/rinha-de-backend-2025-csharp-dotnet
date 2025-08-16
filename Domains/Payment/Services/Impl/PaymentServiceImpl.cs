using PaymentBroker.Domains.Payment.dtos;

namespace PaymentBroker.Domains.Payment.Services;

public class PaymentService : IPaymentService
{
	public ReceivePaymentResponseDto ReceivePayment(ReceivePaymentDto receivePaymentDto)
	{
		// create payment and save in db

		// put in processing queue

		// return response
		ReceivePaymentResponseDto receivePaymentResponseDto = new()
		{
			Message = "O pagamento está sendo processado."
		};

		return receivePaymentResponseDto;
	}
}