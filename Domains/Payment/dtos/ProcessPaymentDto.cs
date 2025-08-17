using System.Text.Json.Serialization;

namespace PaymentBroker.Domains.Payment.dtos;

public class ProcessPaymentDto
{
	[JsonPropertyName("correlationId")]
	public string CorrelationId { get; set; } = "";

	[JsonPropertyName("amount")]
	public float Amount { get; set; }

	[JsonPropertyName("requestedAt")]
	public string RequestedAt { get; set; } = "";
}