using Email_Integration_Api.Dto.RequestModel;
using Email_Integration_Api.Dto.ResponseModel;
using Email_Integration_Api.Implemenntation.Service;

namespace Email_Integration_Api.Implemenntation.IService
{
    public interface IEmailService
    {
        Task SendEmailClient(string msg, string title, string email);
        Task<BaseResponse<Guid>> SendNotificationToUserAsync(Profile profile);
        Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request);
    }
}
