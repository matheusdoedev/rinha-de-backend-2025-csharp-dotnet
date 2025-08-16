using Microsoft.EntityFrameworkCore;

namespace PaymentBroker.Domains.Payment.Repositories;

public interface IPaymentRepository
{
	public Task Add(Payment payment);

	public Task<List<Payment>> GetAll();

	public Task<Payment> GetById(string PaymentId);
	public DbSet<Payment> GetDbSet();
	public Task Save();
}