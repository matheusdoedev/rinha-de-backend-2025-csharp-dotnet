namespace PaymentBroker.Domains.Payment.dtos;

public class SendPaymentToWaitingQueueDto
{
	public string CorrelationId { get; set; } = "";
	public float Amount { get; set; }
	public int Attempts { get; set; } = 1;
}