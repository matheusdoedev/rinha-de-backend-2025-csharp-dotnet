namespace PaymentBroker.Domains.Payment;

using System.ComponentModel.DataAnnotations.Schema;
using PaymentBroker.Domains.Payment;

[Table("payment-processing")]
public class PaymentProcessing
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public string Method { get; set; } = "standard";

	[ForeignKey("Payment")]
	public string PaymentId { get; set; } = "";
	public virtual Payment Payment { get; set; } = new();
}

