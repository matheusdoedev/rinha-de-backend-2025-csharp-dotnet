namespace PaymentBroker.Controllers;

using Microsoft.AspNetCore.Mvc;

using PaymentBroker.Domains.Payment.dtos;
using PaymentBroker.Domains.Payment.Services;

[Controller]
[Route("")]
public class PaymentController(IPaymentService paymentService)
{
	private readonly IPaymentService _paymentService = paymentService;

	[HttpPost]
	[Route("/payments")]
	public ReceivePaymentResponseDto PostReceivePayment([FromBody] ReceivePaymentDto receivePaymentDto)
	{
		try
		{
			ReceivePaymentResponseDto receivePaymentResponseDto = _paymentService.ReceivePayment(receivePaymentDto);

			return receivePaymentResponseDto;
		}
		catch (System.Exception)
		{
			throw;
		}
	}
}