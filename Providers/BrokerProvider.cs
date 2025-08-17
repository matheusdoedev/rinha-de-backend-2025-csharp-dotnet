namespace PaymentBroker.Providers;

using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public static class BrokerProvider
{
	private static readonly string BROKER_HOST = Environment.GetEnvironmentVariable("BROKER_HOST") ?? throw new ArgumentException("broker host string env not defined");
	private static readonly int BROKER_PORT = Convert.ToInt32(Environment.GetEnvironmentVariable("BROKER_PORT") ?? throw new ArgumentException("broker port string env not defined"));
	private static readonly string BROKER_USER = Environment.GetEnvironmentVariable("BROKER_USER") ?? throw new ArgumentException("broker user string env not defined");
	private static readonly string BROKER_PASS = Environment.GetEnvironmentVariable("BROKER_PASS") ?? throw new ArgumentException("broker pass string env not defined");
	private static readonly string[] QUEUES = ["waiting", "fallback"];
	private static ConnectionFactory _factory = new();
	private static IConnection _connection;
	private static IChannel _channel;

	public static async Task CreateQueues()
	{
		_factory = new() { HostName = BROKER_HOST, Port = BROKER_PORT, UserName = BROKER_USER, Password = BROKER_PASS };
		_connection = await _factory.CreateConnectionAsync();
		_channel = await _connection.CreateChannelAsync();
		foreach (string queue in QUEUES)
		{
			await _channel.QueueDeclareAsync(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
		}
	}

	public static async Task SendMessage<T>(string queue, T message)
	{
		byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

		await _channel.BasicPublishAsync("", queue, body);
	}

	public static AsyncEventingBasicConsumer CreateConsumer()
	{
		return new AsyncEventingBasicConsumer(_channel);
	}

	public static IChannel GetChannel()
	{
		return _channel;
	}

	public static T DeserializeMessage<T>(dynamic ea)
	{
		byte[] body = ea.Body.ToArray();
		string? message = Encoding.UTF8.GetString(body);
		T? data = JsonSerializer.Deserialize<T>(message);

		if (data == null)
		{
			throw new InvalidOperationException("it was not possible deserialize this message: " + message);
		}
		return data;
	}
}