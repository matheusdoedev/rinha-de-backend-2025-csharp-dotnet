using Microsoft.EntityFrameworkCore;
using PaymentBroker.Domains.Payment;

namespace PaymentBroker.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Payment> Payments { get; set; }
	public DbSet<PaymentProcessing> PaymentsProcessings { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
			optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION"));
	}
}