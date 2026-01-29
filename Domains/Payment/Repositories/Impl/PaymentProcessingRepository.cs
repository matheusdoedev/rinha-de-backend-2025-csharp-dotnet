namespace PaymentBroker.Domains.Payment.Repositories;

using Microsoft.EntityFrameworkCore;
using PaymentBroker.Contexts;

public class PaymentProcessingRepository(AppDbContext context) : IPaymentProcessingRepository
{
	private readonly AppDbContext _context = context;

	public async Task Add(PaymentProcessing paymentProcessing)
	{
		await _context.AddAsync(paymentProcessing);
	}

	public async Task<List<PaymentProcessing>> GetAll()
	{
		List<PaymentProcessing> paymentsProcessings = await _context.PaymentsProcessings.ToListAsync();

		return paymentsProcessings;
	}

	public async Task<PaymentProcessing> GetById(string PaymentId)
	{
		PaymentProcessing paymentProcessing = await _context.PaymentsProcessings.Where(p => p.Id == PaymentId).FirstAsync();

		return paymentProcessing;
	}

	public async Task Save()
	{
		await _context.SaveChangesAsync();
	}

	public DbSet<PaymentProcessing> GetDbSet()
	{
		return _context.PaymentsProcessings;
	}
}