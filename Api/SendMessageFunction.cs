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
			response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
			response.WriteString("Couldn't read a message from the Request: " + ex.Message);
			return new(response);
		}
		string name = contactMessage.Name;
		string email = contactMessage.Email;
		string message = $"Message from: {name}, ({email})" + Environment.NewLine + contactMessage.Message;

		var mail = new SendGridMessage();
		mail.AddTo(new EmailAddress(Environment.GetEnvironmentVariable("SendGridEmailTo"), Environment.GetEnvironmentVariable("SendGridEmailToName")));
		mail.SetFrom(new EmailAddress(Environment.GetEnvironmentVariable("SendGridEmailFrom"), Environment.GetEnvironmentVariable("SendGridEmailFromName")));
		mail.AddContent("text/html", message);
		mail.SetSubject($"Website Message: {name}");

		return new(OkResult(req), mail);
	}

	private HttpResponseData OkResult(HttpRequestData req)
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

		//[SendGridOutput(ApiKey = "AzureSendGridApiKey")]
		public string MessageJson { get; private set; }

		public HttpResponseData HttpResponse { get; private set; }
	}
}
