namespace PaymentBroker.Jobs;

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

public class FallbackQueueHandlingJob(ILogger<WaitingQueueHandlingJob> logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
	private readonly ILogger<WaitingQueueHandlingJob> _logger = logger;
	private readonly int TIME_DELAY = Convert.ToInt32(Environment.GetEnvironmentVariable("FALLBACK_JOB_TIME") ?? throw new ArgumentException("waiting job time env not defined"));
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

			_paymentProcessor = paymentProcessorFactory.CreateProcessor("fallback");

			await HandleWaitingPayments();
			await Task.Delay(TIME_DELAY, stoppingToken);
		}
	}

	private async Task HandleWaitingPayments()
	{
		bool isPaymentProcessorAvailable = await IsPaymentProcessorAvailable();

		if (!isPaymentProcessorAvailable) return;
		_consumer.ReceivedAsync += async (model, ea) =>
		{
			Thread thread = new(() =>
			{
				HandlePayment(ea).GetAwaiter().GetResult();
			});

			thread.Start();
		};
		await _channel.BasicConsumeAsync(queue: "fallback", autoAck: false, consumer: _consumer);
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

	private async Task HandlePayment(dynamic ea)
	{
		SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto = BrokerProvider.DeserializeMessage<SendPaymentToWaitingQueueDto>(ea);

		await SendToPaymentProcessor(sendPaymentToWaitingQueueDto, ea);
	}

	private async Task SendToPaymentProcessor(SendPaymentToWaitingQueueDto sendPaymentToWaitingQueueDto, dynamic ea)
	{
		try
		{
			using IServiceScope scope = serviceScopeFactory.CreateScope();
			IPaymentProcessorFactory paymentProcessorFactory = scope.ServiceProvider.GetRequiredService<IPaymentProcessorFactory>();

			_paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
			_paymentProcessingRepository = scope.ServiceProvider.GetRequiredService<IPaymentProcessingRepository>();
			_paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
			_paymentProcessor = paymentProcessorFactory.CreateProcessor("waiting");

			Payment payment = await _paymentRepository.GetDbSet().Where(p => p.CorrelationId == sendPaymentToWaitingQueueDto.CorrelationId).FirstAsync();
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
				Method = "fallback",
			};

			await _paymentProcessingRepository.Add(paymentProcessing);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.Message, ex);
			await _paymentService.ResendPaymentToFallbackQueue(sendPaymentToWaitingQueueDto);
		}
	}
}
