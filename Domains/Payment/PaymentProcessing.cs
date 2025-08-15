namespace PaymentBroker.Domains.Payment;

using System.ComponentModel.DataAnnotations.Schema;
using PaymentBroker.Domains.Payment;

[Table("payment-processing")]
public class PaymentProcessing
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public ProcessingMethod ProcessingMethod { get; set; } = ProcessingMethod.STANDARD;
	public PaymentProcessingStatus Status { get; set; } = PaymentProcessingStatus.IN_PROGRESS;
}

