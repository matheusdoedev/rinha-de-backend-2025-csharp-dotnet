namespace PaymentBroker.Domains.Payment.dtos;

public class SendPaymentToWaitingQueueDto
{
	public string PaymentId { get; set; } = "";
	public int Attempts { get; set; } = 1;
}