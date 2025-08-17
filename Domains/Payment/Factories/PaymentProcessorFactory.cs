using PaymentBroker.Domains.Payment.Services;

namespace PaymentBroker.Domains.Payment.Factories;

public interface IPaymentProcessorFactory
{
	public IPaymentProcessor CreateProcessor(string queue);
}