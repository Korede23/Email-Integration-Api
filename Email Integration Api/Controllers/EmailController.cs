using Email_Integration_Api.Implemenntation.IService;
using Email_Integration_Api.Implemenntation.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Email_Integration_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
            private readonly IEmailService _emailService;

            public EmailController(IEmailService emailService)
            {
                _emailService = emailService;
            }

            [HttpPost("send-notification")]
            public async Task<IActionResult> SendNotification([FromBody] Profile profile)
            {
                var response = await _emailService.SendNotificationToUserAsync(profile);
                if (response.Success)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
        }
    
}
