using System.Threading.Tasks;
using Flurl.Http;
using ShitheadApi.Models;
using ShitheadApi.Utilities.Enums;

namespace ShitheadApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IRequestService _requestService;

        public EmailService(IRequestService requestService)
        {
            _requestService = requestService;
        }

        /// <inheritdoc/>
        public async Task<DataBundel<Email>> SendEmail(Email email)
        {
            var request = _requestService.CreateRequest("SendEmail", "api/Email", ApiDomainType.NotificationApi);
            return await request.PostJsonAsync(email).ReceiveJson<DataBundel<Email>>();
        }
    }
}
