using System.Text.Json.Serialization;

namespace PaymentBroker.Domains.Payment.dtos;

public class GetPaymentsSummaryParamsDto
{
	[JsonPropertyName("from")]
	public string From { get; set; } = "";

	[JsonPropertyName("to")]
	public string To { get; set; } = "";
}