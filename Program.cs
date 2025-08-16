using PaymentBroker.Contexts;

using NLog;
using DotEnv.Core;

using PaymentBroker.Domains.Payment.Services;
using PaymentBroker.Domains.Payment.Repositories;

new EnvLoader().Load();

Logger logger = LogManager.LoadConfiguration("nlog.config").GetCurrentClassLogger();
string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? throw new ArgumentException("db connection string env not defined");
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddAuthorization();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);

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