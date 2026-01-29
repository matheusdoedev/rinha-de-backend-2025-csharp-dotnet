using PaymentBroker.Domains.Payment.Services;

namespace PaymentBroker.Domains.Payment.Factories;

public class PaymentProcessorFactoryImpl : IPaymentProcessorFactory
{
	public IPaymentProcessor CreateProcessor(string queue)
	{
		return queue switch
		{
			"fallback" => new PaymentProcessorFallbackImpl(),
			_ => new PaymentProcessorImpl(),
		};
	}
}