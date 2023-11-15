using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ApiIsolated;

public class HttpTrigger
{
	private readonly ILogger _logger;

	public HttpTrigger(ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<HttpTrigger>();
	}

	[Function("ping")]
	public HttpResponseData Run([HttpTrigger(AuthorizationLevel.User, "get")] HttpRequestData req)
	{
		_logger.Log(LogLevel.Information, "API was pinged");
		return req.CreateResponse(HttpStatusCode.OK); ;
	}
}
