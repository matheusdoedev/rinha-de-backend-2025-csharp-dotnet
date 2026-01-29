using Microsoft.EntityFrameworkCore;

namespace PaymentBroker.Domains.Payment.Repositories;

public interface IPaymentProcessingRepository
{
	public Task Add(PaymentProcessing paymentProcessing);

	public Task<List<PaymentProcessing>> GetAll();

	public Task<PaymentProcessing> GetById(string PaymentProcessingId);
	public DbSet<PaymentProcessing> GetDbSet();
	public Task Save();
}