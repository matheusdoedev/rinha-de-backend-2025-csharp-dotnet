using System.Text.Json.Serialization;

namespace PaymentBroker.Domains.Payment.dtos;

/*
	POST /payments
	{
			"correlationId": "4a7901b8-7d26-4d9d-aa19-4dc1c7cf60b3",
			"amount": 19.90
	}
*/
public class ReceivePaymentDto
{
	[JsonPropertyName("correlationId")]
	public string CorrelationId { get; set; } = "";

	[JsonPropertyName("amount")]
	public float Amount { get; set; } = 0;
}