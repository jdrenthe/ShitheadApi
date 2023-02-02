using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShitheadApi.Models;
using ShitheadApi.Services;

namespace ShitheadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NotificationController : Controller
    {
        private readonly IEmailService _emailSender;

        public NotificationController(IEmailService emailSender)
        {
            _emailSender = emailSender;
        }

        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<DataBundel<Email>> SendEmail(Email email)
        {
            var errorMessage = "Couldn't send email.";
            DataBundel<Email> errorResult = new(null, false, errorMessage);

            try
            {
                if (email == null) return errorResult;

                await _emailSender.SendEmail(email);
                return new(null, true, null);
            }
            catch
            {
                return errorResult;
            }
        }

        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<DataBundel<List<Email>>> SendEmails(List<Email> emails)
        {
            var errorMessage = "Couldn't send emails.";
            DataBundel<List<Email>> errorResult = new(null, false, errorMessage);

            try
            {
                if (emails == null || emails.Count < 1) return errorResult;

                //TODO: use cc or bcc
                foreach (var email in emails) await _emailSender.SendEmail(email);
                return new(null, true, null);
            }
            catch
            {
                return errorResult;
            }
        }
    }
}
