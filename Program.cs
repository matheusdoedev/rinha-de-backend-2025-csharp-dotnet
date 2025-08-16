using PaymentBroker.Contexts;

using NLog;
using DotEnv.Core;

using PaymentBroker.Domains.Payment.Services;
using PaymentBroker.Domains.Payment.Repositories;
using PaymentBroker.Providers;

new EnvLoader().Load();

string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? throw new ArgumentException("db connection string env not defined");
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
Logger logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "Payment Broker",
		Version = "v1.0.0",
		Description = "A API that behaves as a middleware between payments APIs. The goal is to intermediate payments."
	});
});
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddAuthorization();

await BrokerProvider.CreateQueues();

try
{
	WebApplication app = builder.Build();

	app.UseSwagger();
	app.UseSwaggerUI();
	app.MapControllers();

	await app.RunAsync();
}
catch (Exception exception)
{
	logger.Error(exception, "Application stopped due to an exception");
	throw;
}
finally
{
	LogManager.Shutdown();
}