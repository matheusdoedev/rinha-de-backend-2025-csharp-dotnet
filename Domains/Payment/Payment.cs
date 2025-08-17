namespace PaymentBroker.Domains.Payment;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("payments")]
public class Payment
{
	[Key]
	public string Id { get; set; } = Guid.NewGuid().ToString();
	public string CorrelationId { get; set; } = "";
	public float Amount { get; set; }
	public string Status { get; set; } = "pending";
}