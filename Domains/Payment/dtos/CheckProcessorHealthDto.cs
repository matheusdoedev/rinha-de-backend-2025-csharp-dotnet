using System.Text.Json.Serialization;

namespace PaymentBroker.Domains.Payment.dtos;

public class CheckProcessorHealthResponseDto
{
	[JsonPropertyName("failing")]
	public bool Failing { get; set; } = false;

	[JsonPropertyName("minResponseTime")]
	public int MinResponseTime { get; set; }
}