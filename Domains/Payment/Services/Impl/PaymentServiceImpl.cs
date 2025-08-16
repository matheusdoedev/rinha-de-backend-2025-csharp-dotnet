using System.Threading.Tasks;
using PaymentBroker.Domains.Payment.dtos;
using PaymentBroker.Domains.Payment.Repositories;

namespace PaymentBroker.Domains.Payment.Services;

public class PaymentService(IPaymentRepository paymentRepository) : IPaymentService
{
	private readonly IPaymentRepository _paymentRepository = paymentRepository;

	public async Task<ReceivePaymentResponseDto> ReceivePayment(ReceivePaymentDto receivePaymentDto)
	{
		await SavePayment(receivePaymentDto);



		// put in processing queue

		// return response
		ReceivePaymentResponseDto receivePaymentResponseDto = new()
		{
			Message = "O pagamento está sendo processado."
		};

		return receivePaymentResponseDto;
	}

	private async Task SavePayment(ReceivePaymentDto receivePaymentDto)
	{
		Payment payment = new()
		{
			CorrelationId = receivePaymentDto.CorrelationId,
			Amount = receivePaymentDto.Amount,
		};

		await _paymentRepository.Add(payment);
		await _paymentRepository.Save();
	}
}