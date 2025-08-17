using System.Text.Json.Serialization;
using PaymentBroker.Domains.Payment.Models;

namespace PaymentBroker.Domains.Payment.dtos;

public class GetPaymentsSummaryResponseDto
{
	[JsonPropertyName("default")]
	public PaymentProcessorMethodSummary Default { get; set; } = new();

	[JsonPropertyName("fallback")]
	public PaymentProcessorMethodSummary Fallback { get; set; } = new();
}