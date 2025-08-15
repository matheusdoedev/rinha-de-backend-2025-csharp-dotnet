using Microsoft.EntityFrameworkCore;

namespace PaymentBroker.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
		if (!optionsBuilder.IsConfigured)
			optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION"));
	}
}