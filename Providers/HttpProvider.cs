using System.Text;
using System.Text.Json;

namespace PaymentBroker.Providers;

public static class HttpProvider
{
	public static async Task<T> SendGetRequest<T>(string apiUrl)
	{
		using HttpClient client = new();

		try
		{
			HttpResponseMessage response = await client.GetAsync(apiUrl);

			response.EnsureSuccessStatusCode();

			string responseBody = await response.Content.ReadAsStringAsync();
			T? responseData = JsonSerializer.Deserialize<T>(responseBody);

			if (responseData == null)
			{
				throw new InvalidOperationException("it was not possible deserialize response data: " + responseData);
			}
			return responseData;
		}
		catch
		{
			throw;
		}
	}

	public static async Task<T> SendPostRequest<U, T>(string apiUrl, U data)
	{
		using HttpClient client = new();
		string? jsonPayload = JsonSerializer.Serialize(data);
		StringContent? content = new(jsonPayload, Encoding.UTF8, "application/json");

		try
		{
			HttpResponseMessage response = await client.PostAsync(apiUrl, content);

			response.EnsureSuccessStatusCode();

			string responseBody = await response.Content.ReadAsStringAsync();
			T? responseData = JsonSerializer.Deserialize<T>(responseBody);

			if (responseData == null)
			{
				throw new InvalidOperationException("it was not possible deserialize response data: " + responseData);
			}
			return responseData;
		}
		catch
		{
			throw;
		}
	}
}