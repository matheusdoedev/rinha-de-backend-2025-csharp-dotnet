namespace PaymentBroker.Domains.Payment.Repositories;

using Microsoft.EntityFrameworkCore;
using PaymentBroker.Contexts;

public class PaymentRepository(AppDbContext context) : IPaymentRepository
{
	private readonly AppDbContext _context = context;

	public async Task Add(Payment payment)
	{
		await _context.AddAsync(payment);
	}

	public async Task<List<Payment>> GetAll()
	{
		List<Payment> payments = await _context.Payments.ToListAsync();

		return payments;
	}

	public async Task<Payment> GetById(string PaymentId)
	{
		Payment payment = await _context.Payments.Where(p => p.Id == PaymentId).FirstAsync();

		return payment;
	}

	public async Task Save()
	{
		await _context.SaveChangesAsync();
	}

	public DbSet<Payment> GetDbSet()
	{
		return _context.Payments;
	}

	public async Task Update(Payment payment)
	{
		_context.Payments.Update(payment);
		await _context.SaveChangesAsync();
	}
}