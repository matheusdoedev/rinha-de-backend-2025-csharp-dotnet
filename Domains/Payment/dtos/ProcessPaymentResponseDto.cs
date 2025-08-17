using System.Text.Json.Serialization;

namespace PaymentBroker.Domains.Payment.dtos;

public class ProcessPaymentResponseDto
{
	[JsonPropertyName("message")]
	public string Message { get; set; } = "";
}