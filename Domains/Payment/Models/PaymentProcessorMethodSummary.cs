using System.Text.Json.Serialization;

namespace PaymentBroker.Domains.Payment.Models;

public class PaymentProcessorMethodSummary
{
	[JsonPropertyName("totalRequests")]
	public int TotalRequests { get; set; }

	[JsonPropertyName("totalAmount")]
	public float TotalAmount { get; set; }
}