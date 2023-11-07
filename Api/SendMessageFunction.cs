using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ApiIsolated;

public class SendMessageFunction
{
	private readonly ILogger _logger;

	public SendMessageFunction(ILoggerFactory loggerFactory)
	{
		_logger = loggerFactory.CreateLogger<SendMessageFunction>();
	}

	[Function("SendMessage")]
	public async Task<RunReturn> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
	{
		_logger.LogInformation("C# HTTP trigger function processed a request.");

		ContactMessage contactMessage;
		try
		{
			contactMessage = await req.ReadFromJsonAsync<ContactMessage>();
		}
		catch (Exception ex)
		{
			HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
			response.Headers.Add("Content-Type", "text/html; charset=utf-8");
			response.WriteString("Couldn't read a message from the Request: " + ex.Message);
			return new(response);
		}
		string name = contactMessage.Name;
		string email = contactMessage.Email;
		string plainMessage = @$"Message from: {name}, ({email})

Message content:
{contactMessage.Message}";
		string htmlMessge = $"<p>Message from: {name}, ({email})</p><p>Message content:</p><p>{contactMessage.Message}</p>";

		var mail = new SendGridMessage()
		{
			From = new EmailAddress(Environment.GetEnvironmentVariable("SendGridEmailFrom"), Environment.GetEnvironmentVariable("SendGridEmailFromName")),
			ReplyTo = new(email, name),
			Subject = $"Website Message: {name}",
		};
		mail.AddTo(new EmailAddress(Environment.GetEnvironmentVariable("SendGridEmailTo"), Environment.GetEnvironmentVariable("SendGridEmailToName")));
		mail.AddContent("text/plain", plainMessage);
		mail.AddContent("text/html", htmlMessge);

		return new(OkResult(req), mail);
	}

	private static HttpResponseData OkResult(HttpRequestData req)
	{
		HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
		response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
		response.WriteString("Email sent");
		return response;
	}

	public class RunReturn
	{
		public RunReturn(HttpResponseData httpResponse, SendGridMessage message = null)
		{
			HttpResponse = httpResponse;
			//Serialize here because otherwise there's a confusion with formats
			MessageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);
		}

		[SendGridOutput(ApiKey = "AzureSendGridApiKey")]
		public string MessageJson { get; private set; }

		public HttpResponseData HttpResponse { get; private set; }
	}
}
