using Microsoft.AspNetCore.Mvc;

namespace PaymentBroker.Controllers;

[ApiController]
[Route("")]
public class HealthCheckController {
	[HttpGet]
	public string GetHealthCheck() {
		return DateTime.UtcNow.ToLongDateString();
	}
}