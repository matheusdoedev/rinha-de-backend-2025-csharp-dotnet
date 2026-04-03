using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PaymentBroker.Domains.Payment;
using PaymentBroker.Domains.Payment.dtos;
using PaymentBroker.Domains.Payment.Factories;
using PaymentBroker.Domains.Payment.Repositories;
using PaymentBroker.Domains.Payment.Services;
using PaymentBroker.Providers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentBroker.Jobs;

public class WaitingQueueHandlingJob(ILogger<WaitingQueueHandlingJob> logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
	private readonly ILogger<WaitingQueueHandlingJob> _logger = logger;
	private readonly int TIME_DELAY = Convert.ToInt32(Environment.GetEnvironmentVariable("WAITING_JOB_TIME") ?? throw new ArgumentException("waiting job time env not defined"));
	private readonly int MAX_ATTEMPTS = Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_WAITING_ATTEMPTS") ?? throw new ArgumentException("max waiting attempts env not defined"));
	private readonly int ACCEPTABLE_RESPONSE_TIME = Convert.ToInt32(Environment.GetEnvironmentVariable("ACCEPTABLE_RESPONSE_TIME") ?? throw new ArgumentException("acceptable response time env not defined"));
	private readonly AsyncEventingBasicConsumer _consumer = BrokerProvider.CreateConsumer();
	private readonly IChannel _channel = BrokerProvider.GetChannel(); // To refactor later, exist a little coupling between this job and rabbitmq lib (broker provider). Find a way to abstract that dependecy inside provider.
	private readonly IServiceScope _serviceScope = serviceScopeFactory.CreateScope();
	private IPaymentRepository _paymentRepository;
	private IPaymentProcessingRepository _paymentProcessingRepository;
	private IPaymentService _paymentService;
	private IPaymentProcessor _paymentProcessor;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			if (_logger.IsEnabled(LogLevel.Information))
			{
				_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
			}

			using IServiceScope scope = serviceScopeFactory.CreateScope();
			IPaymentProcessorFactory paymentProcessorFactory = scope.ServiceProvider.GetRequiredService<IPaymentProcessorFactory>();

			_paymentProcessor = paymentProcessorFactory.CreateProcessor("waiting");

			await HandleWaitingPayments();
			await Task.Delay(TIME_DELAY, stoppingToken);
		}
	}

	private async Task HandleWaitingPayments()
	{
		bool isPaymentProcessorAvailable = await IsPaymentProcessorAvailable();

		if (!isPaymentProcessorAvailable) return;
		_consumer.ReceivedAsync += HandlePayment;
		await _channel.BasicConsumeAsync(queue: "waiting", autoAck: true, consumer: _consumer);
	}

	private async Task<bool> IsPaymentProcessorAvailable()
	{
		try
		{
			CheckProcessorHealthResponseDto checkProcessorHealthResponseDto = await _paymentProcessor.CheckProcessorHealth();

			return !checkProcessorHealthResponseDto.Failing && checkProcessorHealthResponseDto.MinResponseTime <= ACCEPTABLE_RESPONSE_TIME;
		}
		catch
		{
			return false;
		}
	}

	private async Task HandlePayment(dynamic ea, object model)
	{
		SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto = BrokerProvider.DeserializeMessage<SendPaymentToWaitingQueueDto>(ea);

		await SendToPaymentProcessor(sendPaymentToWaitingQueueDto);
	}

	private async Task SendToPaymentProcessor(SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto)
	{
		try
		{
			using IServiceScope scope = serviceScopeFactory.CreateScope();
			IPaymentProcessorFactory paymentProcessorFactory = scope.ServiceProvider.GetRequiredService<IPaymentProcessorFactory>();

			_paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
			_paymentProcessingRepository = scope.ServiceProvider.GetRequiredService<IPaymentProcessingRepository>();
			_paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
			_paymentProcessor = paymentProcessorFactory.CreateProcessor("waiting");

			bool exists = await _paymentRepository.GetDbSet().Where(p => p.CorrelationId == sendPaymentToWaitingQueueDto.CorrelationId).AnyAsync();

			if (exists)
			{
				return;
			}

			Payment payment = new()
			{
				CorrelationId = sendPaymentToWaitingQueueDto.CorrelationId,
				Amount = sendPaymentToWaitingQueueDto.Amount,
			};

			await _paymentRepository.Add(payment);
			await _paymentRepository.Save(); ;

			ProcessPaymentDto processPaymentDto = new()
			{
				Amount = payment.Amount,
				CorrelationId = payment.CorrelationId,
				RequestedAt = DateTime.UtcNow.ToString("o")
			};
			ProcessPaymentResponseDto processPaymentResponseDto = await _paymentProcessor.ProcessPayment(processPaymentDto);
			PaymentProcessing paymentProcessing = new()
			{
				Payment = payment,
				PaymentId = payment.Id,
				Method = "standard",
			};

			await _paymentProcessingRepository.Add(paymentProcessing);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message, ex);
			sendPaymentToWaitingQueueDto.Attempts += 1;
			if (sendPaymentToWaitingQueueDto.Attempts > MAX_ATTEMPTS)
			{
				await _paymentService.ResendPaymentToFallbackQueue(sendPaymentToWaitingQueueDto);
			}
			else
			{
				await _paymentService.ResendPaymentToWaitingQueue(sendPaymentToWaitingQueueDto);
			}
		}
	}
}
