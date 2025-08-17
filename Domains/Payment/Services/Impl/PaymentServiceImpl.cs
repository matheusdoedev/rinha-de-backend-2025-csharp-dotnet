using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PaymentBroker.Domains.Payment.dtos;
using PaymentBroker.Domains.Payment.Models;
using PaymentBroker.Domains.Payment.Repositories;
using PaymentBroker.Providers;

namespace PaymentBroker.Domains.Payment.Services;

public class PaymentService(IPaymentRepository paymentRepository, IPaymentProcessingRepository paymentProcessingRepository) : IPaymentService
{
	private readonly IPaymentRepository _paymentRepository = paymentRepository;
	private readonly IPaymentProcessingRepository _paymentProcessingRepository = paymentProcessingRepository;

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

	public async Task<GetPaymentsSummaryResponseDto> GetPaymentsSummary(GetPaymentsSummaryParamsDto getPaymentsSummaryParamsDto)
	{
		DateTime fromDate = DateTime.Parse(getPaymentsSummaryParamsDto.From, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
		DateTime toDate = DateTime.Parse(getPaymentsSummaryParamsDto.To, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
		GetPaymentsSummaryResponseDto responseDto = new()
		{
			Default = await CalculateSummary("standard", fromDate, toDate),
			Fallback = await CalculateSummary("fallback", fromDate, toDate)
		};

		return responseDto;
	}

	private async Task<PaymentProcessorMethodSummary> CalculateSummary(string method, DateTime from, DateTime to)
	{
		List<PaymentProcessing> paymentProcessings = await _paymentProcessingRepository.
																									GetDbSet()
																									.Include(pp => pp.Payment)
																									.Where(pp => pp.Method == method)
																									.Where(pp => pp.CreatedAt < to && pp.CreatedAt >= from)
																									.ToListAsync();
		PaymentProcessorMethodSummary paymentProcessorMethodSummary = new()
		{
			TotalRequests = paymentProcessings.Count
		};

		foreach (PaymentProcessing paymentProcessing in paymentProcessings)
		{
			paymentProcessorMethodSummary.TotalAmount += paymentProcessing.Payment.Amount;
		}
		return paymentProcessorMethodSummary;
	}
}