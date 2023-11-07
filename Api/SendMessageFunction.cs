using BlazorApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SendGrid;
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
	public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
	{
		_logger.LogInformation("C# HTTP trigger function processed a request.");

		ContactMessage contactMessage;
		try
		{
			contactMessage = await req.ReadFromJsonAsync<ContactMessage>();
		}
		catch (Exception ex)
		{
			return await CreateResponse(req, HttpStatusCode.BadRequest, "Couldn't read a message from the Request: " + ex.Message, false, false);
		}
		string senderName = contactMessage.Name;
		string senderEmail = contactMessage.Email;
		string noreplyEmail = Environment.GetEnvironmentVariable("SendGridEmailFrom");
		string noreplyName = Environment.GetEnvironmentVariable("SendGridEmailFromName");
		string recieverEmail = Environment.GetEnvironmentVariable("SendGridEmailTo");
		string recieverName = Environment.GetEnvironmentVariable("SendGridEmailToName");

		var client = new SendGridClient(Environment.GetEnvironmentVariable("AzureSendGridApiKey"));
		try
		{
			SendGridMessage messageMail = createMessageMail();
			//await client.SendEmailAsync(messageMail);
		}
		catch (Exception ex)
		{
			return await CreateResponse(req, HttpStatusCode.InternalServerError, ex.Message, false, false);
		}

		try
		{
			SendGridMessage confirmationMail = createConfirmationMail();
			//await client.SendEmailAsync(confirmationMail);
		}
		catch (Exception ex)
		{
			return await CreateResponse(req, HttpStatusCode.InternalServerError, ex.Message, true, false);
		}

		return await CreateResponse(req, HttpStatusCode.OK, "Emails sent", true, true);

		SendGridMessage createMessageMail()
		{
			string plainMessage = @$"Message from: {senderName}, ({senderEmail})

Message content:
{contactMessage.Message}";
			string htmlMessge = $"<p>Message from: {senderName}, ({senderEmail})</p><p>Message content:</p><p>{contactMessage.Message}</p>";


			var mail = new SendGridMessage()
			{
				From = new EmailAddress(noreplyEmail, noreplyName),
				ReplyTo = new(senderEmail, senderName),
				Subject = $"Website Message: {senderName}",
			};
			mail.AddTo(new EmailAddress(recieverEmail, recieverName));
			mail.AddContent("text/plain", plainMessage);
			mail.AddContent("text/html", htmlMessge);
			return mail;
		}

		SendGridMessage createConfirmationMail()
		{
			string plainMessage = @$"Thank you for getting in touch {senderName}.

This is an automatic confirmation. I'll respond as soon as I can, Will";
			string htmlMessge = $"<p>Thank you for getting in touch {senderName}.</p><p>This is an automatic confirmation. I'll respond as soon as I can,</p><p>Will</p>";


			var mail = new SendGridMessage()
			{
				From = new EmailAddress(noreplyEmail, noreplyName),
				Subject = $"Thanks for reaching out",
			};
			mail.AddTo(new EmailAddress(senderEmail, senderName));
			mail.AddContent("text/plain", plainMessage);
			mail.AddContent("text/html", htmlMessge);
			return mail;
		}
	}

	private static async Task<HttpResponseData> CreateResponse(HttpRequestData req, HttpStatusCode statusCode, string responseString, bool messageSent, bool confirmationSent)
	{
		HttpResponseData response = req.CreateResponse(statusCode);
		await response.WriteAsJsonAsync(new ContactResponse() { MessageSent = messageSent, ConfirmationSent = confirmationSent, ResponseString = responseString });
		return response;
	}
}
