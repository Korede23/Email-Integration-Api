using Email_Integration_Api.Dto.RequestModel;
using Email_Integration_Api.Dto.ResponseModel;
using Email_Integration_Api.Implemenntation.IService;
using Email_Integration_Api.Model.Entity;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Email_Integration_Api.Implemenntation.Service
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _hostenv;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly string _apiKey;

        public EmailService(IWebHostEnvironment hostenv, IOptions<EmailConfiguration> emailConfiguration, IConfiguration configuration)
        {
            _hostenv = hostenv;
            _emailConfiguration = emailConfiguration.Value;
            _apiKey = configuration.GetValue<string>("MailConfig:mailApikey");
        }

        public async Task<BaseResponse<Guid>> SendNotificationToUserAsync(Profile profile)
        {
            var mailRecieverRequestDto = new MailRecieverDto
            {
                Email = profile.Email,
                Name = profile.FirstName + " " + profile.LastName,
            };

            string emailBody = $"<p>Hello {profile.FirstName},</p>\r\n" +
                                $"<p>Welcome to Korede Hotel management! We’re happy to have you join our system.</p>\r\n" +
                                $"<p>You're highly welcome to our Hotel</p>";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "WELCOME TO Email Test",
                HtmlContent = emailBody,
                ToEmail = "oseniahmadkorede@gmail.com"
            };

            try
            {
                await SendEmailAsync(mailRecieverRequestDto, mailRequest);
                return new BaseResponse<Guid>
                {
                    Message = "Notification sent successfully",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Guid>
                {
                    Message = $"Failed to send notification: {ex.Message}",
                    Success = false,
                };
            }
        }

        public async Task SendEmailClient(string msg, string title, string email)
        {
            if (string.IsNullOrEmpty(msg))
            {
                throw new ArgumentNullException(nameof(msg), "Email message content cannot be null or empty");
            }

            var message = new MimeMessage();
            message.To.Add(MailboxAddress.Parse(email));
            message.From.Add(new MailboxAddress(_emailConfiguration.EmailSenderName, _emailConfiguration.EmailSenderAddress));
            message.Subject = title;

            message.Body = new TextPart("html")
            {
                Text = msg
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    Console.WriteLine("Inside email client");
                    client.Connect(_emailConfiguration.SMTPServerAddress, _emailConfiguration.SMTPServerPort, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfiguration.EmailSenderAddress, _emailConfiguration.EmailSenderPassword);
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred in email client: {ex.Message}");
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        public async Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request)
        {
            try
            {
                Console.WriteLine("Calling email client");
                string buildContent = $"Dear {model.Name},<p>{request.Body}</p>";

                if (string.IsNullOrWhiteSpace(request.HtmlContent))
                {
                    throw new ArgumentNullException(nameof(request.HtmlContent), "Email content cannot be null or empty");
                }

                await SendEmailClient(request.HtmlContent, request.Title, model.Email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending email: {ex.Message}");
                throw new Exception("There was an error while sending email", ex);
            }
        }
    }
}
